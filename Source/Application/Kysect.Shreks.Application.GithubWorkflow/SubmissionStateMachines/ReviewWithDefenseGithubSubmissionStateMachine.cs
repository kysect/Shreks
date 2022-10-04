using Kysect.Shreks.Application.Commands.Commands;
using Kysect.Shreks.Application.Commands.Processors;
using Kysect.Shreks.Application.Commands.Result;
using Kysect.Shreks.Application.GithubWorkflow.Abstractions;
using Kysect.Shreks.Application.GithubWorkflow.Abstractions.Models;
using Kysect.Shreks.Application.GithubWorkflow.Extensions;
using Kysect.Shreks.Application.GithubWorkflow.Submissions;
using Kysect.Shreks.Application.Validators;
using Kysect.Shreks.Common.Resources;
using Kysect.Shreks.Core.Models;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Core.Users;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Application.GithubWorkflow.SubmissionStateMachines;

public class ReviewWithDefenseGithubSubmissionStateMachine : IGithubSubmissionStateMachine
{
    private readonly GithubSubmissionService _githubSubmissionService;
    private readonly SubmissionService _shreksCommandProcessor;
    private readonly ShreksCommandProcessor _commandProcessor;
    private readonly ILogger _logger;
    private readonly IPullRequestEventNotifier _eventNotifier;
    private readonly IPermissionValidator _permissionValidator;

    public ReviewWithDefenseGithubSubmissionStateMachine(
        SubmissionService shreksCommandProcessor,
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

    public async Task ProcessPullRequestReviewApprove(IShreksCommand? command, GithubPullRequestDescriptor prDescriptor, User sender)
    {
        await _permissionValidator.EnsureUserIsOrganizationMentor(sender.Id, prDescriptor.Organization);

        await ChangeSubmissionState(SubmissionState.Reviewed, prDescriptor, sender);

        if (command is not null)
        {
            BaseShreksCommandResult result = await _commandProcessor.ProcessBaseCommandSafe(command, CancellationToken.None);
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
            BaseShreksCommandResult result = await _commandProcessor.ProcessBaseCommandSafe(command, CancellationToken.None);
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
        await ChangeSubmissionState(SubmissionState.Active, prDescriptor, sender);
    }

    public async Task ProcessPullRequestClosed(bool isMerged, GithubPullRequestDescriptor prDescriptor, User sender)
    {
        Submission submission;

        bool isOrganizationMentor = await _permissionValidator.IsOrganizationMentor(sender.Id, prDescriptor.Organization);

        string? message = null;
        if (isOrganizationMentor)
        {
            if (isMerged)
            {
                submission = await _githubSubmissionService.GetLastSubmissionByPr(prDescriptor);
                if (submission.Points is null)
                {
                    message = UserCommandProcessingMessage.MentorMergeUnratedSubmission();
                    BaseShreksCommandResult commandResult = await _commandProcessor.ProcessBaseCommandSafe(new RateCommand(100, 0), CancellationToken.None);
                    if (!commandResult.IsSuccess)
                        message = commandResult.Message;
                }
                else
                {
                    message = UserCommandProcessingMessage.MergePullRequestAndMarkAsCompleted();
                }
            }
            else
            {
                submission = await ChangeSubmissionState(SubmissionState.Inactive, prDescriptor, sender);
                message = UserCommandProcessingMessage.ClosePullRequestWithUnratedSubmission(submission.Code);
            }
        }
        else
        {
            SubmissionState state = isMerged ? SubmissionState.Completed : SubmissionState.Inactive;
            submission = await ChangeSubmissionState(state, prDescriptor, sender);

            if (isMerged && submission.Points is null)
            {
                message = UserCommandProcessingMessage.MergePullRequestWithoutRate(submission.Code);
                await _eventNotifier.SendCommentToPullRequest(message);
            }
        }

        if (message is not null)
            await _eventNotifier.SendCommentToPullRequest(message);
    }

    private async Task<Submission> ChangeSubmissionState(SubmissionState state, GithubPullRequestDescriptor githubPullRequestDescriptor, User sender)
    {
        Submission submission = await _githubSubmissionService.GetLastSubmissionByPr(githubPullRequestDescriptor);
        Submission completedSubmission = await _shreksCommandProcessor.UpdateSubmissionState(submission.Id, sender.Id, state, CancellationToken.None);

        await _eventNotifier.NotifySubmissionUpdate(completedSubmission, _logger);
        return completedSubmission;
    }
}