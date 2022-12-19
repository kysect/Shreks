using Kysect.Shreks.Application.Abstractions.Permissions;
using Kysect.Shreks.Application.Abstractions.Submissions;
using Kysect.Shreks.Application.Commands.Commands;
using Kysect.Shreks.Application.Commands.Processors;
using Kysect.Shreks.Application.Commands.Result;
using Kysect.Shreks.Application.Dto.Submissions;
using Kysect.Shreks.Application.GithubWorkflow.Abstractions;
using Kysect.Shreks.Application.GithubWorkflow.Abstractions.Models;
using Kysect.Shreks.Application.GithubWorkflow.Submissions;
using Kysect.Shreks.Common.Resources;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Core.Users;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Application.GithubWorkflow.SubmissionStateMachines;

public class GithubSubmissionStateMachineAdapter : IGithubSubmissionStateMachine
{
    private readonly ISubmissionWorkflowService _submissionWorkflowService;
    private readonly GithubSubmissionService _githubSubmissionService;
    private readonly ShreksCommandProcessor _commandProcessor;
    private readonly IPullRequestEventNotifier _eventNotifier;
    private readonly ILogger _logger;
    private readonly IPermissionValidator _permissionValidator;

    public GithubSubmissionStateMachineAdapter(
        ISubmissionWorkflowService submissionWorkflowService,
        GithubSubmissionService githubSubmissionService,
        ShreksCommandProcessor commandProcessor,
        IPullRequestEventNotifier eventNotifier,
        ILogger logger,
        IPermissionValidator permissionValidator)
    {
        _submissionWorkflowService = submissionWorkflowService;
        _githubSubmissionService = githubSubmissionService;
        _commandProcessor = commandProcessor;
        _eventNotifier = eventNotifier;
        _logger = logger;
        _permissionValidator = permissionValidator;
    }

    public async Task ProcessPullRequestReviewApprove(
        IShreksCommand? command,
        GithubPullRequestDescriptor prDescriptor,
        User sender)
    {
        Submission submission = await _githubSubmissionService.GetLastSubmissionByPr(prDescriptor);
        ISubmissionWorkflow workflow = await _submissionWorkflowService.GetWorkflowAsync(submission.Id, default);

        SubmissionActionMessageDto messageDto = await workflow.SubmissionApprovedAsync(
            sender.Id, submission.Id, default);

        if (command is not null)
        {
            BaseShreksCommandResult result = await _commandProcessor.ProcessBaseCommandSafe(command, default);
            await _eventNotifier.SendCommentToPullRequest(result.Message);
            _logger.LogInformation("Notify: {Message}", result.Message);

            return;
        }

        await _eventNotifier.SendCommentToPullRequest(messageDto.Message);
        _logger.LogInformation("Notify: {Message}", messageDto.Message);
    }

    public async Task ProcessPullRequestReviewRequestChanges(
        IShreksCommand? command,
        GithubPullRequestDescriptor prDescriptor,
        User user)
    {
        Submission submission = await _githubSubmissionService.GetLastSubmissionByPr(prDescriptor);
        ISubmissionWorkflow workflow = await _submissionWorkflowService.GetWorkflowAsync(submission.Id, default);

        SubmissionActionMessageDto messageDto = await workflow.SubmissionNotAcceptedAsync(
            user.Id, submission.Id, default);

        await _eventNotifier.SendCommentToPullRequest(messageDto.Message);
        _logger.LogInformation("Notify: {Message}", messageDto.Message);

        if (command is not null)
        {
            BaseShreksCommandResult result = await _commandProcessor.ProcessBaseCommandSafe(command, default);
            await _eventNotifier.SendCommentToPullRequest(result.Message);
            _logger.LogInformation("Notify: {Message}", result.Message);
        }
    }

    public async Task ProcessPullRequestReviewComment(IShreksCommand? command)
    {
        if (command is not null)
        {
            BaseShreksCommandResult result = await _commandProcessor.ProcessBaseCommandSafe(command, default);

            await _eventNotifier.SendCommentToPullRequest(result.Message);
            _logger.LogInformation("Notify: {Message}", result.Message);
        }

        if (command is not RateCommand)
        {
            string message = UserCommandProcessingMessage.ReviewWithoutRate();
            await _eventNotifier.SendCommentToPullRequest(message);
            _logger.LogInformation("Notify: {Message}", message);
        }
    }

    public async Task ProcessPullRequestReopen(GithubPullRequestDescriptor prDescriptor, User sender)
    {
        Submission submission = await _githubSubmissionService.GetLastSubmissionByPr(prDescriptor);
        ISubmissionWorkflow workflow = await _submissionWorkflowService.GetWorkflowAsync(submission.Id, default);

        SubmissionActionMessageDto messageDto = await workflow.SubmissionReactivatedAsync(
            sender.Id, submission.Id, default);

        await _eventNotifier.SendCommentToPullRequest(messageDto.Message);
        _logger.LogInformation("Notify: {Message}", messageDto.Message);
    }

    public async Task ProcessPullRequestClosed(bool isMerged, GithubPullRequestDescriptor prDescriptor, User sender)
    {
        Submission submission = await _githubSubmissionService.GetLastSubmissionByPr(prDescriptor);
        ISubmissionWorkflow workflow = await _submissionWorkflowService.GetWorkflowAsync(submission.Id, default);

        bool isOrganizationMentor = await _permissionValidator.IsOrganizationMentor(
            sender.Id, prDescriptor.Organization);

        SubmissionActionMessageDto message = (isOrganizationMentor, isMerged) switch
        {
            (true, true) => await workflow.SubmissionAcceptedAsync(sender.Id, submission.Id, default),
            (true, false) => await workflow.SubmissionRejectedAsync(sender.Id, submission.Id, default),
            (false, _) => await workflow.SubmissionAbandonedAsync(sender.Id, submission.Id, isMerged, default),
        };

        await _eventNotifier.SendCommentToPullRequest(message.Message);
        _logger.LogInformation("Notify: {Message}", message.Message);
    }
}