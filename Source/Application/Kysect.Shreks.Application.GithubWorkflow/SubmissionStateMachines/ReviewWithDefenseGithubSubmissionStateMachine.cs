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
using Kysect.Shreks.DataAccess.Abstractions;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Application.GithubWorkflow.SubmissionStateMachines;

public class ReviewWithDefenseGithubSubmissionStateMachine : IGithubSubmissionStateMachine
{
    private readonly IShreksDatabaseContext _context;
    private readonly GithubSubmissionService _githubSubmissionService;
    private readonly SubmissionService _shreksCommandProcessor;
    private readonly ShreksCommandProcessor _commandProcessor;
    private readonly ILogger _logger;
    private readonly IPullRequestEventNotifier _eventNotifier;

    public ReviewWithDefenseGithubSubmissionStateMachine(
        IShreksDatabaseContext context,
        SubmissionService shreksCommandProcessor,
        ShreksCommandProcessor commandProcessor,
        ILogger logger,
        IPullRequestEventNotifier eventNotifier)
    {
        _githubSubmissionService = new GithubSubmissionService(context);
        _context = context;
        _shreksCommandProcessor = shreksCommandProcessor;
        _commandProcessor = commandProcessor;
        _logger = logger;
        _eventNotifier = eventNotifier;
    }

    public async Task ProcessPullRequestReviewApprove(IShreksCommand? command, GithubPullRequestDescriptor prDescriptor)
    {
        await ChangeSubmissionState(SubmissionState.Reviewed, prDescriptor);

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

    public async Task ProcessPullRequestReopen(bool? isMerged, GithubPullRequestDescriptor prDescriptor)
    {
        if (isMerged.HasValue && isMerged == false)
            await ChangeSubmissionState(SubmissionState.Active, prDescriptor);
    }

    public async Task ProcessPullRequestClosed(bool isMerged, GithubPullRequestDescriptor prDescriptor)
    {
        Submission submission;

        User sender = await _context.UserAssociations.GetUserByGithubUsername(prDescriptor.Sender);
        bool isOrganizationMentor = await PermissionValidator.IsOrganizationMentor(_context, sender.Id, prDescriptor.Organization);

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
                submission = await ChangeSubmissionState(SubmissionState.Inactive, prDescriptor);
                message = UserCommandProcessingMessage.ClosePullRequestWithUnratedSubmission(submission.Code);
            }
        }
        else
        {
            SubmissionState state = isMerged ? SubmissionState.Completed : SubmissionState.Inactive;
            submission = await ChangeSubmissionState(state, prDescriptor);

            if (isMerged && submission.Points is null)
            {
                message = UserCommandProcessingMessage.MergePullRequestWithoutRate(submission.Code);
                await _eventNotifier.SendCommentToPullRequest(message);
            }
        }

        if (message is not null)
            await _eventNotifier.SendCommentToPullRequest(message);
    }

    private async Task<Submission> ChangeSubmissionState(SubmissionState state, GithubPullRequestDescriptor githubPullRequestDescriptor)
    {
        User user = await _context.UserAssociations.GetUserByGithubUsername(githubPullRequestDescriptor.Sender);

        Submission submission = await _githubSubmissionService.GetLastSubmissionByPr(githubPullRequestDescriptor);
        Submission completedSubmission = await _shreksCommandProcessor.UpdateSubmissionState(submission.Id, user.Id, state, CancellationToken.None);

        await _eventNotifier.NotifySubmissionUpdate(completedSubmission, _logger);
        return completedSubmission;
    }
}