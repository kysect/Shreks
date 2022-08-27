using Kysect.Shreks.Application.Abstractions.Github.Commands;
using Kysect.Shreks.Application.Abstractions.Github.Queries;
using Kysect.Shreks.Application.Commands.Commands;
using Kysect.Shreks.Application.Commands.Parsers;
using Kysect.Shreks.Application.Dto.Github;
using Kysect.Shreks.Integration.Github.Client;
using Kysect.Shreks.Integration.Github.ContextFactory;
using Kysect.Shreks.Integration.Github.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Octokit;
using Octokit.Webhooks;
using Octokit.Webhooks.Events;
using Octokit.Webhooks.Events.IssueComment;
using Octokit.Webhooks.Events.PullRequest;
using Octokit.Webhooks.Events.PullRequestReview;
using Octokit.Webhooks.Models;
using PullRequestReviewEvent = Octokit.Webhooks.Events.PullRequestReviewEvent;

namespace Kysect.Shreks.Integration.Github.Processors;

public sealed class ShreksWebhookEventProcessor : WebhookEventProcessor
{
    private readonly IActionNotifier _actionNotifier;
    private readonly ILogger<ShreksWebhookEventProcessor> _logger;
    private readonly IShreksCommandParser _commandParser;
    private readonly IMediator _mediator;
    private readonly IOrganizationGithubClientProvider _clientProvider;


    public ShreksWebhookEventProcessor(
        IActionNotifier actionNotifier, 
        ILogger<ShreksWebhookEventProcessor> logger,
        IShreksCommandParser commandParser, 
        IMediator mediator,
        IOrganizationGithubClientProvider clientProvider)
    {
        _actionNotifier = actionNotifier;
        _logger = logger;
        _commandParser = commandParser;
        _mediator = mediator;
        _clientProvider = clientProvider;
    }

    protected override async Task ProcessPullRequestWebhookAsync(WebhookHeaders headers,
        PullRequestEvent pullRequestEvent, PullRequestAction action)
    {
        _logger.LogDebug($"{nameof(ProcessPullRequestWebhookAsync)}: {pullRequestEvent.GetType().Name}");

        if (IsSenderBotOrNull(pullRequestEvent))
        {
            _logger.LogTrace($"{nameof(ProcessPullRequestWebhookAsync)} skipped because sender is bot or null");
            return;
        }

        switch (action)
        {
            case PullRequestActionValue.Synchronize:
            case PullRequestActionValue.Opened:

                CancellationToken cancellationToken = CancellationToken.None;

                string login = pullRequestEvent.Sender!.Login;
                string payload = pullRequestEvent.PullRequest.DiffUrl;
                string organization = pullRequestEvent.Organization!.Login;
                string repository = pullRequestEvent.Repository!.Name;
                string branch = pullRequestEvent.PullRequest.Head.Ref;
                long prNum = pullRequestEvent.PullRequest.Number;

                var pullRequestDescriptor = new GithubPullRequestDescriptor(
                    login,
                    payload,
                    organization,
                    repository,
                    branch,
                    prNum);

                var subjectCourseId = await GetSubjectCourseByOrganization(organization, cancellationToken);
                var userId = await GetUserByGithubLogin(login, cancellationToken);
                var assignmentId = await GetAssignemntByBranchAndSubjectCourse(branch, subjectCourseId, cancellationToken);

                var command = new CreateOrUpdateGithubSubmission.Command(userId, assignmentId, pullRequestDescriptor);

                var response = await _mediator.Send(command, cancellationToken);
                if (response.IsCreated)
                {
                    await _actionNotifier.SendComment(
                        pullRequestEvent,
                        prNum,
                        $"Created submission with id {response.Submission.Id}");
                }
                else
                {
                    await _actionNotifier.SendCommitComment(
                        pullRequestEvent,
                        pullRequestEvent.PullRequest.Head.Sha,
                        $"Updated submission with id {response.Submission.Id}");
                }

                break;
            case PullRequestActionValue.Reopened:
                break;
        }
    }

    protected override async Task ProcessPullRequestReviewWebhookAsync(WebhookHeaders headers,
        PullRequestReviewEvent pullRequestReviewEvent, PullRequestReviewAction action)
    {
        _logger.LogDebug($"{nameof(ProcessPullRequestReviewWebhookAsync)}: {pullRequestReviewEvent.GetType().Name}");

        if (IsSenderBotOrNull(pullRequestReviewEvent))
        {
            _logger.LogTrace($"{nameof(ProcessPullRequestReviewWebhookAsync)} skipped because sender is bot or null");
            return;
        }

        switch (action)
        {
            case PullRequestReviewActionValue.Submitted:
                break;
            case PullRequestReviewActionValue.Edited:
                break;
            case PullRequestReviewActionValue.Dismissed:
                break;
        }

        await _actionNotifier.ApplyInComments(
            pullRequestReviewEvent,
            pullRequestReviewEvent.PullRequest.Number,
            nameof(ProcessPullRequestWebhookAsync));
    }

    protected override async Task ProcessIssueCommentWebhookAsync(WebhookHeaders headers,
        IssueCommentEvent issueCommentEvent, IssueCommentAction action)
    {
        _logger.LogDebug($"{nameof(ProcessIssueCommentWebhookAsync)}: {issueCommentEvent.GetType().Name}");

        if (IsSenderBotOrNull(issueCommentEvent))
        {
            _logger.LogTrace($"{nameof(ProcessIssueCommentWebhookAsync)} skipped because sender is bot or null");
            return;
        }

        if (!IsPullRequestCommand(issueCommentEvent))
        {
            _logger.LogTrace($"Skip commit in {issueCommentEvent.Issue.Id}. Issue comments is not supported.");
            return;
        }

        GithubPullRequestDescriptor pullRequestDescriptor = await GetPullRequestDescriptor(issueCommentEvent);

        switch (action)
        {
            case IssueCommentActionValue.Edited:
                break;
            case IssueCommentActionValue.Created:
                try
                {
                    var comment = issueCommentEvent.Comment.Body;
                    if (comment.FirstOrDefault() == '/')
                    {
                        IShreksCommand command = _commandParser.Parse(comment);
                        var contextCreator = new PullRequestCommentContextFactory(_mediator, pullRequestDescriptor);
                        var processor = new GithubCommandProcessor(contextCreator, CancellationToken.None);

                        var result = await command.AcceptAsync(processor);

                        if (!string.IsNullOrEmpty(result.Message))
                        {
                            await _actionNotifier.SendComment(
                                issueCommentEvent,
                                issueCommentEvent.Issue.Number,
                                result.Message);
                        }

                        await _actionNotifier.ReactInComments(
                            issueCommentEvent,
                            issueCommentEvent.Comment.Id,
                            result.IsSuccess);
                    }
                }
                catch(Exception e)
                {
                    await _actionNotifier.SendComment(
                        issueCommentEvent,
                        issueCommentEvent.Issue.Number,
                        e.Message);
                }
                break;
            case IssueCommentActionValue.Deleted:
                break;
        }
    }

    private bool IsPullRequestCommand(IssueCommentEvent issueCommentEvent)
    {
        return issueCommentEvent.Issue.PullRequest.Url is not null;
    }

    private async Task<GithubPullRequestDescriptor> GetPullRequestDescriptor(IssueCommentEvent issueCommentEvent)
    {
        ArgumentNullException.ThrowIfNull(issueCommentEvent.Sender);
        ArgumentNullException.ThrowIfNull(issueCommentEvent.Organization);
        ArgumentNullException.ThrowIfNull(issueCommentEvent.Repository);

        GitHubClient gitHubClient = await _clientProvider.GetClient(issueCommentEvent.Organization.Login);
        PullRequest pullRequest = await gitHubClient.PullRequest.Get(issueCommentEvent.Repository.Id, (int)issueCommentEvent.Issue.Number);

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
    
    private async Task<Guid> GetAssignemntByBranchAndSubjectCourse(string branch, Guid subjectCourseId,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new GetAssignmentByBranchAndSubjectCourse.Query(branch,
                subjectCourseId));
        return response.AssignmentId;
    }

    private bool IsSenderBotOrNull(WebhookEvent webhookEvent) =>
        webhookEvent.Sender is null || webhookEvent.Sender.Type == UserType.Bot;
}