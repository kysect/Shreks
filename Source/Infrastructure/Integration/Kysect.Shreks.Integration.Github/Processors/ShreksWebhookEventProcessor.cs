using Kysect.Shreks.Application.Abstractions.Github.Commands;
using Kysect.Shreks.Application.Abstractions.Github.Queries;
using Kysect.Shreks.Application.Abstractions.Submissions.Commands;
using Kysect.Shreks.Application.Abstractions.Submissions.Queries;
using Kysect.Shreks.Application.Commands.Commands;
using Kysect.Shreks.Application.Commands.Parsers;
using Kysect.Shreks.Application.Commands.Result;
using Kysect.Shreks.Application.Dto.Github;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Integration.Github.Applicaiton;
using Kysect.Shreks.Integration.Github.ContextFactory;
using MediatR;
using Microsoft.Extensions.Logging;
using Octokit.Webhooks.Events;
using Octokit.Webhooks.Events.IssueComment;
using Octokit.Webhooks.Events.PullRequest;
using Octokit.Webhooks.Events.PullRequestReview;

namespace Kysect.Shreks.Integration.Github.Processors;

public class ShreksWebhookEventProcessor
{
    private readonly IShreksCommandParser _commandParser;
    private readonly IMediator _mediator;

    public ShreksWebhookEventProcessor(IShreksCommandParser commandParser, IMediator mediator)
    {
        _commandParser = commandParser;
        _mediator = mediator;
    }

    public async Task ProcessPullRequestWebhookAsync(
        PullRequestEvent pullRequestEvent,
        PullRequestAction action,
        GithubPullRequestDescriptor pullRequestDescriptor,
        ILogger repositoryLogger,
        IPullRequestCommitEventNotifier pullRequestCommitEventNotifier)
    {
        string pullRequestAction = action;

        switch (pullRequestAction)
        {
            case PullRequestActionValue.Synchronize:
            case PullRequestActionValue.Opened:
                CancellationToken cancellationToken = CancellationToken.None;

                var subjectCourseId = await GetSubjectCourseByOrganization(pullRequestDescriptor.Organization, cancellationToken);
                var userId = await GetUserByGithubLogin(pullRequestDescriptor.Sender, cancellationToken);
                var assignmentId = await GetAssignmentByBranchAndSubjectCourse(pullRequestDescriptor.BranchName, subjectCourseId);

                var command = new CreateOrUpdateGithubSubmission.Command(userId, assignmentId, pullRequestDescriptor);

                (bool isCreated, SubmissionDto? submissionDto) = await _mediator.Send(command, cancellationToken);
                if (isCreated)
                {
                    string message = $"Submission {submissionDto.Code} ({submissionDto.SubmissionDate}) was created.";
                    await pullRequestCommitEventNotifier.SendCommentToPullRequest(message);
                }
                else
                {
                    await NotifySubmissionUpdate(submissionDto, repositoryLogger, pullRequestCommitEventNotifier, sendCommitComment: true);
                }

                break;
            case PullRequestActionValue.Reopened when pullRequestEvent.PullRequest.Merged is false:
                await ChangeSubmissionState(SubmissionStateDto.Active, pullRequestDescriptor, repositoryLogger, pullRequestCommitEventNotifier);
                break;
            case PullRequestActionValue.Closed:
                var merged = pullRequestEvent.PullRequest.Merged ?? false;
                var state = merged ? SubmissionStateDto.Completed : SubmissionStateDto.Inactive;
                var submission = await ChangeSubmissionState(state, pullRequestDescriptor, repositoryLogger, pullRequestCommitEventNotifier);

                if (merged && submission.Points is null)
                {
                    string message = $"Warning: pull request was merged, but submission {submission.Code} is not yet rated.";
                    await pullRequestCommitEventNotifier.SendCommentToPullRequest(message);
                }

                break;

            default:
                repositoryLogger.LogWarning($"Unsupported pull request webhook type was received: {pullRequestAction}");
                break;
        }
    }

    private async Task<SubmissionDto> ChangeSubmissionState(
        SubmissionStateDto state,
        GithubPullRequestDescriptor githubPullRequestDescriptor,
        ILogger repositoryLogger,
        IPullRequestCommitEventNotifier pullRequestCommitEventNotifier)
    {
        var user = await GetUserByGithubLogin(githubPullRequestDescriptor.Sender, CancellationToken.None);
        var submission = await GetGithubSubmissionAsync(
            githubPullRequestDescriptor.Sender,
            githubPullRequestDescriptor.Repository,
            githubPullRequestDescriptor.PrNumber);

        var competeSubmissionCommand = new UpdateSubmissionState.Command(user, submission.Id, state);

        var response = await _mediator.Send(competeSubmissionCommand, CancellationToken.None);
        var pullRequestCommentEventNotifier = pullRequestCommitEventNotifier;
        await NotifySubmissionUpdate(response.Submission, repositoryLogger, pullRequestCommentEventNotifier, sendComment: true);

        return response.Submission;
    }

    public async Task ProcessPullRequestReviewWebhookAsync(
        PullRequestReviewEvent pullRequestReviewEvent,
        PullRequestReviewAction action,
        GithubPullRequestDescriptor pullRequestDescriptor,
        ILogger repositoryLogger,
        IPullRequetsEventNotifier pullRequestEventNotifier)
    {
        IShreksCommand command = null;
        string comment;
        BaseShreksCommandResult result;
        string pullRequestReviewAction = action;

        switch (pullRequestReviewAction)
        {
            case PullRequestReviewActionValue.Submitted when pullRequestReviewEvent.Review.State == "approved":
                comment = pullRequestReviewEvent.Review.Body ?? string.Empty;
                if (comment.FirstOrDefault() == '/')
                {
                    command = _commandParser.Parse(comment);
                }
                else
                {
                    command = new RateCommand(100, 0);
                }

                var getSubmissionCommand = new GetLastSubmissionByPr.Query(pullRequestDescriptor);

                var response = await _mediator.Send(getSubmissionCommand, CancellationToken.None);

                switch (response.Submission.Points)
                {
                    case null:
                        result = await ProceedCommandAsync(command, pullRequestDescriptor, repositoryLogger);
                        await pullRequestEventNotifier.SendCommentToPullRequest(result.Message);
                        repositoryLogger.LogInformation("Notify: " + result.Message);
                        break;
                    case 100:
                    {
                        var message = "Review successfully processed, but submission already has 100 points";
                        await pullRequestEventNotifier.SendCommentToPullRequest(message);
                        repositoryLogger.LogInformation("Notify: " + message);
                        break;
                    }
                    default:
                    {
                        var message = $"Submission is already rated with {response.Submission.Points} points. If you want to change it, please use /update command.";
                        await pullRequestEventNotifier.SendCommentToPullRequest(message);
                        repositoryLogger.LogInformation("Notify: " + message);
                        break;
                    }
                }


                break;
            case PullRequestReviewActionValue.Submitted when pullRequestReviewEvent.Review.State == "changes_requested":
                comment = pullRequestReviewEvent.Review.Body ?? string.Empty;
                if (comment.FirstOrDefault() == '/')
                {
                    command = _commandParser.Parse(comment);
                }
                else
                {
                    command = new RateCommand(0, 0);
                }

                result = await ProceedCommandAsync(command, pullRequestDescriptor, repositoryLogger);
                await pullRequestEventNotifier.SendCommentToPullRequest(result.Message);
                repositoryLogger.LogInformation("Notify: " + result.Message);
                break;
            case PullRequestReviewActionValue.Submitted when pullRequestReviewEvent.Review.State == "commented":
                comment = pullRequestReviewEvent.Review.Body;
                if (comment is null)
                {
                    repositoryLogger.LogInformation("Review body is null, skipping review comment");
                    break;
                }
                if (comment.FirstOrDefault() == '/')
                {
                    command = _commandParser.Parse(comment);
                    result = await ProceedCommandAsync(command, pullRequestDescriptor, repositoryLogger);
                    await pullRequestEventNotifier.SendCommentToPullRequest(result.Message);
                    repositoryLogger.LogInformation("Notify: " + result.Message);
                }

                if (command is not RateCommand)
                {
                    string message = $"Review proceeded, but submission is not yet rated and still will be presented in queue";
                    await pullRequestEventNotifier.SendCommentToPullRequest(message);
                    repositoryLogger.LogInformation("Notify: " + message);
                }

                break;
            case PullRequestReviewActionValue.Edited:
            case PullRequestReviewActionValue.Dismissed:

                repositoryLogger.LogWarning($"Pull request review action {pullRequestReviewAction} is not supported.");
                break;
            default:
                repositoryLogger.LogWarning("Pull request review for pr {prLink} is not processed.", pullRequestDescriptor.Payload);
                break;
        }
    }

    public async Task ProcessIssueCommentWebhookAsync(
        IssueCommentEvent issueCommentEvent,
        IssueCommentAction action,
        GithubPullRequestDescriptor pullRequestDescriptor,
        ILogger repositoryLogger,
        IPullRequestCommentEventNotifier pullRequestCommentEventNotifier)
    {
        string issueCommentAction = action;
        switch (issueCommentAction)
        {
            case IssueCommentActionValue.Created:
                var comment = issueCommentEvent.Comment.Body;
                if (comment.FirstOrDefault() != '/')
                    break;

                IShreksCommand command = _commandParser.Parse(comment);
                var result = await ProceedCommandAsync(command, pullRequestDescriptor, repositoryLogger);
                if (!string.IsNullOrEmpty(result.Message))
                    await pullRequestCommentEventNotifier.SendCommentToPullRequest(result.Message);

                await pullRequestCommentEventNotifier.ReactToUserComment(result.IsSuccess);

                break;
            case IssueCommentActionValue.Deleted:
            case IssueCommentActionValue.Edited:
                repositoryLogger.LogTrace($"Pull request comment {issueCommentAction} event will be ignored.");
                break;
        }
    }

    private async Task<BaseShreksCommandResult> ProceedCommandAsync(IShreksCommand command, GithubPullRequestDescriptor pullRequestDescriptor, ILogger repositoryLogger)
    {
        var contextCreator = new PullRequestCommentContextFactory(_mediator, pullRequestDescriptor, repositoryLogger);
        var processor = new GithubCommandProcessor(contextCreator, repositoryLogger, CancellationToken.None);
        BaseShreksCommandResult result;

        try
        {
            result = await command.AcceptAsync(processor);
        }
        catch (Exception e)
        {
            const string message = $"An internal error occurred while processing command. Contact support for details.";
            repositoryLogger.LogError(e, message);
            result = new BaseShreksCommandResult(false, message);
        }

        return result;
    }

    private async Task<Guid> GetSubjectCourseByOrganization(string organization, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetSubjectCourseByOrganization.Query(organization));
        return response.SubjectCourseId;
    }

    private async Task<Guid> GetUserByGithubLogin(string login, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetUserByGithubUsername.Query(login));
        return response.UserId;
    }

    private async Task<Guid> GetAssignmentByBranchAndSubjectCourse(string branch, Guid subjectCourseId)
    {
        var response = await _mediator.Send(new GetAssignmentByBranchAndSubjectCourse.Query(branch, subjectCourseId));
        return response.AssignmentId;
    }

    private async Task<SubmissionDto> GetGithubSubmissionAsync(string organization, string repository, long prNumber)
    {
        var query = new GetGithubSubmission.Query(organization, repository, prNumber);
        var response = await _mediator.Send(query);
        return response.Submission;
    }

    public async Task NotifySubmissionUpdate(
        SubmissionDto submission,
        ILogger logger,
        IPullRequestCommitEventNotifier pullRequestCommitEventNotifier,
        bool sendComment = false,
        bool sendCommitComment = false)
    {
        string message = $"Submission {submission.Code} was updated." +
                         $"\nState: {submission.State}" +
                         $"\nDate: {submission.SubmissionDate}";

        if (sendComment)
        {
            logger.LogInformation("Notify comment posted into PR: " + message);
            await pullRequestCommitEventNotifier.SendCommentToPullRequest(message);
        }

        if (sendCommitComment)
        {
            logger.LogInformation("Notify posted as commit comment: " + message);
            await pullRequestCommitEventNotifier.SendCommentToTriggeredCommit(message);
        }
    }
}