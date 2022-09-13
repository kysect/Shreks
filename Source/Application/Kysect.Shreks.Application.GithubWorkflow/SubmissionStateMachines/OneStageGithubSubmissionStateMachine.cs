using Kysect.Shreks.Application.Commands.Commands;
using Kysect.Shreks.Application.Commands.Processors;
using Kysect.Shreks.Application.Commands.Result;
using Kysect.Shreks.Application.Dto.Github;
using Kysect.Shreks.Application.GithubWorkflow.Abstractions;
using Kysect.Shreks.Application.GithubWorkflow.Extensions;
using Kysect.Shreks.Application.GithubWorkflow.Submissions;
using Kysect.Shreks.Core.Models;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Abstractions;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Application.GithubWorkflow.SubmissionStateMachines;

public class OneStageGithubSubmissionStateMachine : IGithubSubmissionStateMachine
{
    private readonly IShreksDatabaseContext _context;
    private readonly GithubSubmissionService _githubSubmissionService;
    private readonly SubmissionService _shreksCommandProcessor;

    public OneStageGithubSubmissionStateMachine(IShreksDatabaseContext context, SubmissionService shreksCommandProcessor)
    {
        _githubSubmissionService = new GithubSubmissionService(context);
        _context = context;
        _shreksCommandProcessor = shreksCommandProcessor;
    }

    public async Task ProcessPullRequestReviewApprove(
        IShreksCommand? command,
        ShreksCommandProcessor commandProcessor,
        GithubPullRequestDescriptor prDescriptor,
        ILogger logger,
        IPullRequestEventNotifier eventNotifier)
    {
        Submission submission = await _githubSubmissionService.GetLastSubmissionByPr(prDescriptor);

        double? points = submission.Points?.Value;

        if (command is not null)
        {
            BaseShreksCommandResult result = await commandProcessor.ProcessBaseCommandSafe(command, CancellationToken.None);
            await eventNotifier.SendCommentToPullRequest(result.Message);
            logger.LogInformation("Notify: " + result.Message);
            return;
        }

        switch (points)
        {
            case null:
                {
                    command = new RateCommand(100, 0);
                    BaseShreksCommandResult result = await commandProcessor.ProcessBaseCommandSafe(command, CancellationToken.None);
                    await eventNotifier.SendCommentToPullRequest(result.Message);
                    logger.LogInformation("Notify: " + result.Message);
                    break;
                }

            case 100:
                {
                    var message = "Review successfully processed, but submission already has 100 points";
                    await eventNotifier.SendCommentToPullRequest(message);
                    logger.LogInformation("Notify: " + message);
                    break;
                }

            default:
                {
                    var message = $"Submission is already rated with {points} points. If you want to change it, please use /update command.";
                    await eventNotifier.SendCommentToPullRequest(message);
                    logger.LogInformation("Notify: " + message);
                    break;
                }
        }
    }

    public async Task ProcessPullRequestReviewRequestChanges(
        IShreksCommand? command,
        ShreksCommandProcessor commandProcessor,
        ILogger logger,
        IPullRequestEventNotifier eventNotifier)
    {
        if (command is null)
            command = new RateCommand(0, 0);

        BaseShreksCommandResult result = await commandProcessor.ProcessBaseCommandSafe(command, CancellationToken.None);
        await eventNotifier.SendCommentToPullRequest(result.Message);
        logger.LogInformation("Notify: " + result.Message);
    }

    public async Task ProcessPullRequestReviewComment(
        IShreksCommand? command,
        ShreksCommandProcessor commandProcessor,
        ILogger logger,
        IPullRequestEventNotifier eventNotifier)
    {
        if (command is not null)
        {
            BaseShreksCommandResult result = await commandProcessor.ProcessBaseCommandSafe(command, CancellationToken.None);
            await eventNotifier.SendCommentToPullRequest(result.Message);
            logger.LogInformation("Notify: " + result.Message);
        }

        if (command is not RateCommand)
        {
            string message = $"Review proceeded, but submission is not yet rated and still will be presented in queue";
            await eventNotifier.SendCommentToPullRequest(message);
            logger.LogInformation("Notify: " + message);
        }
    }

    public async Task ProcessPullRequestReopen(bool? isMerged, GithubPullRequestDescriptor prDescriptor, ILogger logger, IPullRequestCommitEventNotifier eventNotifier)
    {
        if (isMerged.HasValue && isMerged == false)
            await ChangeSubmissionState(SubmissionState.Active, prDescriptor, logger, eventNotifier);
    }

    public async Task ProcessPullRequestClosed(
        bool merged,
        GithubPullRequestDescriptor prDescriptor,
        ILogger logger,
        IPullRequestCommitEventNotifier eventNotifier)
    {
        var state = merged ? SubmissionState.Completed : SubmissionState.Inactive;
        var submission = await ChangeSubmissionState(state, prDescriptor, logger, eventNotifier);

        if (merged && submission.Points is null)
        {
            string message = $"Warning: pull request was merged, but submission {submission.Code} is not yet rated.";
            await eventNotifier.SendCommentToPullRequest(message);
        }
    }

    private async Task<Submission> ChangeSubmissionState(
        SubmissionState state,
        GithubPullRequestDescriptor githubPullRequestDescriptor,
        ILogger repositoryLogger,
        IPullRequestCommitEventNotifier pullRequestCommitEventNotifier)
    {
        User user = await _context.UserAssociations.GetUserByGithubUsername(githubPullRequestDescriptor.Sender);

        Submission submission = await _githubSubmissionService.GetLastSubmissionByPr(githubPullRequestDescriptor);
        Submission completedSubmission = await _shreksCommandProcessor.UpdateSubmissionState(submission.Id, user.Id, state, CancellationToken.None);

        await pullRequestCommitEventNotifier.NotifySubmissionUpdate(completedSubmission, repositoryLogger, true, false);
        return completedSubmission;
    }
}