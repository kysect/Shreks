using Kysect.Shreks.Application.Abstractions.Github.Commands;
using Kysect.Shreks.Application.Abstractions.Github.Queries;
using Kysect.Shreks.Application.Abstractions.Submissions.Commands;
using Kysect.Shreks.Application.Abstractions.Submissions.Queries;
using Kysect.Shreks.Application.Commands.Commands;
using Kysect.Shreks.Application.Commands.Parsers;
using Kysect.Shreks.Application.Commands.Result;
using Kysect.Shreks.Application.Dto.Github;
using Kysect.Shreks.Application.Dto.Study;
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
    private readonly ShreksWebhookCommentSender _commentSender;

    public ShreksWebhookEventProcessor(ShreksWebhookCommentSender commentSender, IShreksCommandParser commandParser, IMediator mediator)
    {
        _commandParser = commandParser;
        _mediator = mediator;
        _commentSender = commentSender;
    }

    public async Task ProcessPullRequestWebhookAsync(
        PullRequestEvent pullRequestEvent,
        PullRequestAction action,
        GithubPullRequestDescriptor pullRequestDescriptor,
        ILogger repositoryLogger)
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
                    await _commentSender.NotifySubmissionCreated(pullRequestEvent, submissionDto, repositoryLogger);
                else
                    await _commentSender.NotifySubmissionUpdate(pullRequestEvent, submissionDto, repositoryLogger, sendCommitComment: true);

                break;
            case PullRequestActionValue.Reopened when pullRequestEvent.PullRequest.Merged is false:
                await ChangeSubmissionState(pullRequestEvent, SubmissionStateDto.Active, pullRequestDescriptor, repositoryLogger);
                break;
            case PullRequestActionValue.Closed:
                var merged = pullRequestEvent.PullRequest.Merged ?? false;
                var state = merged ? SubmissionStateDto.Completed : SubmissionStateDto.Inactive;
                var submission = await ChangeSubmissionState(pullRequestEvent, state, pullRequestDescriptor, repositoryLogger);

                if (merged && submission.Points is null)
                    await _commentSender.WarnPullRequestMergedWithoutPoints(pullRequestEvent, submission, repositoryLogger);
                break;

            default:
                repositoryLogger.LogWarning($"Unsupported pull request webhook type was received: {pullRequestAction}");
                break;
        }
    }

    private async Task<SubmissionDto> ChangeSubmissionState(
        PullRequestEvent pullRequestEvent,
        SubmissionStateDto state,
        GithubPullRequestDescriptor githubPullRequestDescriptor,
        ILogger repositoryLogger)
    {
        var user = await GetUserByGithubLogin(pullRequestEvent.Sender!.Login, CancellationToken.None);
        var submission = await GetGithubSubmissionAsync(
            pullRequestEvent.Organization!.Login,
            pullRequestEvent.Repository!.Name,
            pullRequestEvent.PullRequest.Number);

        var competeSubmissionCommand = new UpdateSubmissionState.Command(user, submission.Id, state);

        var response = await _mediator.Send(competeSubmissionCommand, CancellationToken.None);
        await _commentSender.NotifySubmissionUpdate(pullRequestEvent, response.Submission, repositoryLogger, sendComment: true);

        return response.Submission;
    }

    public async Task ProcessPullRequestReviewWebhookAsync(
        PullRequestReviewEvent pullRequestReviewEvent,
        PullRequestReviewAction action,
        GithubPullRequestDescriptor pullRequestDescriptor,
        ILogger repositoryLogger)
    {
        IShreksCommand command = null;
        string comment;
        BaseShreksCommandResult result;
        string pullRequestReviewAction = action;
        switch (pullRequestReviewAction)
        {
            case PullRequestReviewActionValue.Submitted when pullRequestReviewEvent.Review.State == "approved":
                comment = pullRequestReviewEvent.Review.Body;
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
                        await _commentSender.NotifyPullRequestReviewProcessed(pullRequestReviewEvent, repositoryLogger, result.Message);
                        break;
                    case 100:
                    {
                        var message = "Review successfully processed, but submission already has 100 points";
                        await _commentSender.NotifyPullRequestReviewProcessed(pullRequestReviewEvent, repositoryLogger, message);
                        break;
                    }
                    default:
                    {
                        var message = $"Submission is already rated with {response.Submission.Points} points. If you want to change it, please use /update command.";
                        await _commentSender.NotifyPullRequestReviewProcessed(pullRequestReviewEvent, repositoryLogger, message);
                        break;
                    }
                }


                break;
            case PullRequestReviewActionValue.Submitted when pullRequestReviewEvent.Review.State == "changes_requested":
                comment = pullRequestReviewEvent.Review.Body;
                if (comment.FirstOrDefault() == '/')
                {
                    command = _commandParser.Parse(comment);
                }
                else
                {
                    command = new RateCommand(0, 0);
                }

                result = await ProceedCommandAsync(command, pullRequestDescriptor, repositoryLogger);
                await _commentSender.NotifyPullRequestReviewProcessed(pullRequestReviewEvent, repositoryLogger, result.Message);
                break;
            case PullRequestReviewActionValue.Submitted when pullRequestReviewEvent.Review.State == "commented":
                comment = pullRequestReviewEvent.Review.Body ?? "";
                if (comment.FirstOrDefault() == '/')
                {
                    command = _commandParser.Parse(comment);
                    result = await ProceedCommandAsync(command, pullRequestDescriptor, repositoryLogger);
                    await _commentSender.NotifyPullRequestReviewProcessed(pullRequestReviewEvent, repositoryLogger, result.Message);
                }

                if (command is not RateCommand)
                {
                    await _commentSender.NotifyPullRequestReviewProcessed(
                        pullRequestReviewEvent,
                        repositoryLogger,
                        $"Review proceeded, but submission is not yet rated and still will be presented in queue");
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
        ILogger repositoryLogger)
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
                await _commentSender.NotifyAboutCommandProcessingResult(issueCommentEvent, result, repositoryLogger);

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
}