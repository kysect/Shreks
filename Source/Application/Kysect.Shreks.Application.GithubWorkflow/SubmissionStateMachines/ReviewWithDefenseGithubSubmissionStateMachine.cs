using Kysect.Shreks.Application.Commands.Commands;
using Kysect.Shreks.Application.Commands.Processors;
using Kysect.Shreks.Application.Commands.Result;
using Kysect.Shreks.Application.GithubWorkflow.Abstractions;
using Kysect.Shreks.Application.GithubWorkflow.Abstractions.Models;
using Kysect.Shreks.Application.GithubWorkflow.Submissions;
using Kysect.Shreks.Application.Validators;
using Kysect.Shreks.Common.Resources;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Core.Users;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Application.GithubWorkflow.SubmissionStateMachines;

public class ReviewWithDefenseGithubSubmissionStateMachine : IGithubSubmissionStateMachine
{
    private readonly GithubSubmissionService _githubSubmissionService;
    private readonly ISubmissionService _shreksCommandProcessor;
    private readonly ShreksCommandProcessor _commandProcessor;
    private readonly ILogger _logger;
    private readonly IPullRequestEventNotifier _eventNotifier;
    private readonly IPermissionValidator _permissionValidator;

    public ReviewWithDefenseGithubSubmissionStateMachine(
        ISubmissionService shreksCommandProcessor,
        ShreksCommandProcessor commandProcessor,
        ILogger logger,
        IPullRequestEventNotifier eventNotifier,
        GithubSubmissionService githubSubmissionService,
        IPermissionValidator permissionValidator)
    {
        _githubSubmissionService = githubSubmissionService;
        _shreksCommandProcessor = shreksCommandProcessor;
        _commandProcessor = commandProcessor;
        _logger = logger;
        _eventNotifier = eventNotifier;
        _permissionValidator = permissionValidator;
    }

    public async Task ProcessPullRequestReviewApprove(
        IShreksCommand? command,
        GithubPullRequestDescriptor prDescriptor,
        User sender)
    {
        await _permissionValidator.EnsureUserIsOrganizationMentor(sender.Id, prDescriptor.Organization);

        Submission submission = await _githubSubmissionService.GetLastSubmissionByPr(prDescriptor);
        await _shreksCommandProcessor.ReviewSubmissionAsync(submission.Id, sender.Id, default);

        if (command is not null)
        {
            BaseShreksCommandResult result = await _commandProcessor.ProcessBaseCommandSafe(command, default);
            await _eventNotifier.SendCommentToPullRequest(result.Message);
            _logger.LogInformation("Notify: " + result.Message);

            return;
        }

        string message = UserCommandProcessingMessage.SubmissionMarkAsReviewedAndNeedDefense();
        await _eventNotifier.SendCommentToPullRequest(message);
        _logger.LogInformation("Notify: " + message);
    }

    public async Task ProcessPullRequestReviewRequestChanges(IShreksCommand? command)
    {
        if (command is null)
            command = new RateCommand(ratingPercent: 0, extraPoints: 0);

        BaseShreksCommandResult result = await _commandProcessor.ProcessBaseCommandSafe(command, CancellationToken.None);
        await _eventNotifier.SendCommentToPullRequest(result.Message);
        _logger.LogInformation("Notify: " + result.Message);
    }

    public async Task ProcessPullRequestReviewComment(IShreksCommand? command)
    {
        if (command is not null)
        {
            BaseShreksCommandResult result = await _commandProcessor.ProcessBaseCommandSafe(command, default);

            await _eventNotifier.SendCommentToPullRequest(result.Message);
            _logger.LogInformation("Notify: " + result.Message);
        }

        if (command is not RateCommand)
        {
            string message = UserCommandProcessingMessage.ReviewWithoutRate();
            await _eventNotifier.SendCommentToPullRequest(message);
            _logger.LogInformation("Notify: " + message);
        }
    }

    public async Task ProcessPullRequestReopen(GithubPullRequestDescriptor prDescriptor, User sender)
    {
        Submission submission = await _githubSubmissionService.GetLastSubmissionByPr(prDescriptor);
        await _shreksCommandProcessor.ActivateSubmissionAsync(submission.Id, sender.Id, default);
        await _eventNotifier.SendCommentToPullRequest(UserCommandProcessingMessage.SubmissionActivatedSuccessfully());
    }

    public async Task ProcessPullRequestClosed(bool isMerged, GithubPullRequestDescriptor prDescriptor, User sender)
    {
        bool isOrganizationMentor = await _permissionValidator.IsOrganizationMentor(
            sender.Id, prDescriptor.Organization);

        string? message = isOrganizationMentor
            ? await ProcessClosedByOrganizationMember(isMerged, prDescriptor, sender)
            : await ProcessClosedByNotOrganizationMember(isMerged, prDescriptor, sender);

        if (message is not null)
            await _eventNotifier.SendCommentToPullRequest(message);
    }

    private async Task<string> ProcessClosedByOrganizationMember(
        bool isMerged,
        GithubPullRequestDescriptor prDescriptor,
        User sender)
    {
        return isMerged
            ? await ProcessClosedMergedByOrganizationMember(prDescriptor)
            : await ProcessClosedNotMergedByOrganizationMember(prDescriptor, sender);
    }

    private async Task<string> ProcessClosedMergedByOrganizationMember(GithubPullRequestDescriptor prDescriptor)
    {
        Submission submission = await _githubSubmissionService.GetLastSubmissionByPr(prDescriptor);

        if (submission.Points is not null)
            return UserCommandProcessingMessage.MergePullRequestAndMarkAsCompleted();

        var command = new RateCommand(100, 0);

        BaseShreksCommandResult commandResult = await _commandProcessor.ProcessBaseCommandSafe(command, default);

        return commandResult.IsSuccess
            ? UserCommandProcessingMessage.MentorMergeUnratedSubmission()
            : commandResult.Message;
    }

    private async Task<string> ProcessClosedNotMergedByOrganizationMember(
        GithubPullRequestDescriptor prDescriptor,
        User sender)
    {
        Submission submission = await _githubSubmissionService.GetLastSubmissionByPr(prDescriptor);
        submission = await _shreksCommandProcessor.DeactivateSubmissionAsync(submission.Id, sender.Id, default);

        return UserCommandProcessingMessage.ClosePullRequestWithUnratedSubmission(submission.Code);
    }

    private async Task<string?> ProcessClosedByNotOrganizationMember(
        bool isMerged,
        GithubPullRequestDescriptor prDescriptor,
        User sender)
    {
        Submission submission = await _githubSubmissionService.GetLastSubmissionByPr(prDescriptor);

        if (isMerged)
        {
            await _shreksCommandProcessor.CompleteSubmissionAsync(submission.Id, sender.Id, default);
        }
        else
        {
            await _shreksCommandProcessor.DeactivateSubmissionAsync(submission.Id, sender.Id, default);
        }

        if (isMerged is false || submission.Points is not null)
            return null;

        string message = UserCommandProcessingMessage.MergePullRequestWithoutRate(submission.Code);
        await _eventNotifier.SendCommentToPullRequest(message);

        return message;
    }
}