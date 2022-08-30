using Kysect.Shreks.Application.Abstractions.Github.Commands;
using Kysect.Shreks.Application.Abstractions.Github.Queries;
using Kysect.Shreks.Application.Abstractions.Submissions.Commands;
using Kysect.Shreks.Application.Abstractions.Submissions.Queries;
using Kysect.Shreks.Application.Commands.Commands;
using Kysect.Shreks.Application.Commands.Parsers;
using Kysect.Shreks.Application.Commands.Result;
using Kysect.Shreks.Application.Dto.Github;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Integration.Github.Client;
using Kysect.Shreks.Integration.Github.ContextFactory;
using MediatR;
using Microsoft.Extensions.Logging;
using Octokit;
using Octokit.Webhooks;
using Octokit.Webhooks.Events;
using Octokit.Webhooks.Events.IssueComment;
using Octokit.Webhooks.Events.PullRequest;
using Octokit.Webhooks.Events.PullRequestReview;
using PullRequestReviewEvent = Octokit.Webhooks.Events.PullRequestReviewEvent;

namespace Kysect.Shreks.Integration.Github.Processors;

public class ShreksWebhookEventProcessor
{
    private readonly ILogger<ShreksWebhookEventProcessorProxy> _logger;
    private readonly IShreksCommandParser _commandParser;
    private readonly IMediator _mediator;
    private readonly IOrganizationGithubClientProvider _clientProvider;
    private readonly ShreksWebhookCommentSender _commentSender;

    public ShreksWebhookEventProcessor(ShreksWebhookCommentSender commentSender, IShreksCommandParser commandParser, IMediator mediator, IOrganizationGithubClientProvider clientProvider, ILogger<ShreksWebhookEventProcessorProxy> logger)
    {
        _logger = logger;
        _commandParser = commandParser;
        _mediator = mediator;
        _clientProvider = clientProvider;
        _commentSender = commentSender;
    }

    public async Task ProcessPullRequestWebhookAsync(
        WebhookHeaders headers,
        PullRequestEvent pullRequestEvent,
        PullRequestAction action)
    {
        string pullRequestAction = action;

        string senderLogin = pullRequestEvent.Sender!.Login;
        string payload = pullRequestEvent.PullRequest.DiffUrl;
        string organization = pullRequestEvent.Organization!.Login;
        string repository = pullRequestEvent.Repository!.Name;
        string branch = pullRequestEvent.PullRequest.Head.Ref;
        long prNum = pullRequestEvent.PullRequest.Number;

        var pullRequestDescriptor = new GithubPullRequestDescriptor(
            senderLogin,
            payload,
            organization,
            repository,
            branch,
            prNum);

        switch (pullRequestAction)
        {
            case PullRequestActionValue.Synchronize:
            case PullRequestActionValue.Reopened:
            case PullRequestActionValue.Opened:

                CancellationToken cancellationToken = CancellationToken.None;

                var subjectCourseId = await GetSubjectCourseByOrganization(organization, cancellationToken);
                var userId = await GetUserByGithubLogin(senderLogin, cancellationToken);
                var assignmentId =
                    await GetAssignemntByBranchAndSubjectCourse(branch, subjectCourseId);

                var command = new CreateOrUpdateGithubSubmission.Command(userId, assignmentId, pullRequestDescriptor);

                (bool isCreated, SubmissionDto? submissionDto) = await _mediator.Send(command, cancellationToken);
                if (isCreated)
                    await _commentSender.NotifySubmissionCreated(pullRequestEvent, submissionDto);
                else
                    await _commentSender.NotifySubmissionUpdate(pullRequestEvent, submissionDto);

                break;

            case PullRequestActionValue.Closed when pullRequestEvent.PullRequest.Merged is true:
                var githubForkSyncer = new GithubForkSyncer(_mediator, _clientProvider, _logger);

                if (await githubForkSyncer.IsTemplateRepo(pullRequestDescriptor))
                {
                    await githubForkSyncer.SyncRepos(pullRequestDescriptor, pullRequestEvent.PullRequest.Base.Ref);
                }


                var user = await GetUserByGithubLogin(pullRequestEvent.Sender!.Login, CancellationToken.None);
                var submission = await GetGithubSubmissionAsync(
                    pullRequestEvent.Organization!.Login,
                    pullRequestEvent.Repository!.Name,
                    pullRequestEvent.PullRequest.Number);

                var competeSubmissionCommand = new UpdateSubmissionState.Command(
                    user, submission.Id, SubmissionStateDto.Completed);

                await _mediator.Send(competeSubmissionCommand, CancellationToken.None);
                break;

            default:
                _logger.LogWarning($"Unsupported pull request webhook type was received: {pullRequestAction}");
                break;
        }
    }

    public async Task ProcessPullRequestReviewWebhookAsync(
        WebhookHeaders headers,
        PullRequestReviewEvent pullRequestReviewEvent,
        PullRequestReviewAction action)
    {
        string pullRequestReviewAction = action;
        switch (pullRequestReviewAction)
        {
            case PullRequestReviewActionValue.Submitted:
            case PullRequestReviewActionValue.Edited:
            case PullRequestReviewActionValue.Dismissed:

                _logger.LogWarning($"Pull request review action {pullRequestReviewAction} is not supported.");
                break;
        }

        await _commentSender.NotifyPullRequestReviewProcessed(pullRequestReviewEvent, action);
    }

    public async Task ProcessIssueCommentWebhookAsync(
        WebhookHeaders headers,
        IssueCommentEvent issueCommentEvent,
        IssueCommentAction action)
    {
        GithubPullRequestDescriptor pullRequestDescriptor = await GetPullRequestDescriptor(issueCommentEvent);

        string issueCommentAction = action;
        switch (issueCommentAction)
        {
            case IssueCommentActionValue.Created:
                var comment = issueCommentEvent.Comment.Body;
                if (comment.FirstOrDefault() == '/')
                {
                    IShreksCommand command = _commandParser.Parse(comment);
                    var contextCreator = new PullRequestCommentContextFactory(_mediator, pullRequestDescriptor, _logger);
                    var processor = new GithubCommandProcessor(contextCreator, _logger, CancellationToken.None);
                    BaseShreksCommandResult result;

                    try
                    {
                        result = await command.AcceptAsync(processor);
                    }
                    catch (Exception e)
                    {
                        string message = $"An error internal error occurred while processing command. Contact support for details.";
                        _logger.LogError(e, message);
                        result = new BaseShreksCommandResult(false, message);
                    }

                    await _commentSender.NotifyAboutCommandProcessingResult(issueCommentEvent, result);
                }

                break;

            case IssueCommentActionValue.Deleted:
            case IssueCommentActionValue.Edited:
                _logger.LogTrace($"Pull request comment {issueCommentAction} event will be ignored.");
                break;
        }
    }

    private async Task<GithubPullRequestDescriptor> GetPullRequestDescriptor(IssueCommentEvent issueCommentEvent)
    {
        ArgumentNullException.ThrowIfNull(issueCommentEvent.Sender);
        ArgumentNullException.ThrowIfNull(issueCommentEvent.Organization);
        ArgumentNullException.ThrowIfNull(issueCommentEvent.Repository);

        GitHubClient gitHubClient = await _clientProvider.GetClient(issueCommentEvent.Organization.Login);
        PullRequest pullRequest = await gitHubClient.PullRequest
            .Get(issueCommentEvent.Repository.Id, (int)issueCommentEvent.Issue.Number);

        return new GithubPullRequestDescriptor(
            issueCommentEvent.Sender.Login,
            Payload: pullRequest.Url,
            issueCommentEvent.Organization.Login,
            issueCommentEvent.Repository.Name,
            BranchName: pullRequest.Head.Ref,
            pullRequest.Number);
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

    private async Task<Guid> GetAssignemntByBranchAndSubjectCourse(string branch, Guid subjectCourseId)
    {
        var response = await _mediator.Send(
            new GetAssignmentByBranchAndSubjectCourse.Query(branch,
                subjectCourseId));
        return response.AssignmentId;
    }

    private async Task<SubmissionDto> GetGithubSubmissionAsync(string organization, string repository, long prNumber)
    {
        var query = new GetGithubSubmission.Query(organization, repository, prNumber);
        var response = await _mediator.Send(query);
        return response.Submission;
    }
}