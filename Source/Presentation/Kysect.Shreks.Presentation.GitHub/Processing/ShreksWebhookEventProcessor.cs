using Kysect.Shreks.Application.GithubWorkflow.Abstractions.Client;
using Kysect.Shreks.Application.GithubWorkflow.Abstractions.Extensions;
using Kysect.Shreks.Application.GithubWorkflow.Abstractions.Models;
using Kysect.Shreks.Presentation.GitHub.Notifiers;
using Microsoft.Extensions.Logging;
using Octokit;
using Octokit.Webhooks;
using Octokit.Webhooks.Events;
using Octokit.Webhooks.Events.IssueComment;
using Octokit.Webhooks.Events.PullRequest;
using Octokit.Webhooks.Events.PullRequestReview;
using Octokit.Webhooks.Models;
using PullRequestReviewEvent = Octokit.Webhooks.Events.PullRequestReviewEvent;

namespace Kysect.Shreks.Presentation.GitHub.Processing;

public class ShreksWebhookEventProcessor : WebhookEventProcessor
{
    private readonly IActionNotifier _actionNotifier;
    private readonly IOrganizationGithubClientProvider _clientProvider;
    private readonly IShreksWebhookEventHandler _handler;
    private readonly ILogger<ShreksWebhookEventProcessor> _logger;

    public ShreksWebhookEventProcessor(
        IActionNotifier actionNotifier,
        ILogger<ShreksWebhookEventProcessor> logger,
        IOrganizationGithubClientProvider clientProvider,
        IShreksWebhookEventHandler eventHandler)
    {
        _actionNotifier = actionNotifier;
        _logger = logger;
        _clientProvider = clientProvider;
        _handler = eventHandler;
    }

    protected override async Task ProcessPullRequestWebhookAsync(
        WebhookHeaders headers,
        PullRequestEvent pullRequestEvent,
        PullRequestAction action)
    {
        GithubPullRequestDescriptor githubPullRequestDescriptor = CreateDescriptor(pullRequestEvent);
        ILogger repositoryLogger = _logger.ToRepositoryLogger(githubPullRequestDescriptor);

        const string methodName = nameof(ProcessPullRequestWebhookAsync);

        if (IsSenderBotOrNull(pullRequestEvent))
        {
            repositoryLogger.LogTrace($"{methodName} was skipped because sender is bot or null");
            return;
        }

        string pullRequestAction = action;

        repositoryLogger.LogInformation(
            "{MethodName}: {EventName} with type {Action}",
            methodName,
            pullRequestEvent.GetType().Name,
            pullRequestAction);

        var pullRequestCommitEventNotifier = new PullRequestCommitEventNotifier(
            _actionNotifier,
            pullRequestEvent,
            pullRequestEvent.PullRequest.Head.Sha,
            pullRequestEvent.PullRequest.Number,
            repositoryLogger);

        try
        {
            switch (pullRequestAction)
            {
                case PullRequestActionValue.Synchronize:
                case PullRequestActionValue.Opened:
                    await _handler.ProcessPullRequestUpdate(
                        githubPullRequestDescriptor,
                        repositoryLogger,
                        pullRequestCommitEventNotifier,
                        default);
                    break;

                case PullRequestActionValue.Reopened:
                    await _handler.ProcessPullRequestReopen(
                        githubPullRequestDescriptor,
                        repositoryLogger,
                        pullRequestCommitEventNotifier,
                        default);
                    break;

                case PullRequestActionValue.Closed:
                    bool merged = pullRequestEvent.PullRequest.Merged ?? false;
                    await _handler.ProcessPullRequestClosed(
                        merged,
                        githubPullRequestDescriptor,
                        repositoryLogger,
                        pullRequestCommitEventNotifier,
                        default);
                    break;

                case PullRequestActionValue.Assigned:
                case PullRequestActionValue.ReviewRequestRemoved:
                case PullRequestActionValue.ReviewRequested:
                    repositoryLogger.LogDebug("Skip pull request action with type {Action}", pullRequestAction);
                    break;

                default:
                    repositoryLogger.LogWarning(
                        "Unsupported pull request webhook type was received: {Action}",
                        pullRequestAction);
                    break;
            }
        }
        catch (Exception e)
        {
            string message = $"Failed to handle {pullRequestAction}";
            repositoryLogger.LogError(e, "{MethodName}: {Message}", methodName, message);

            await pullRequestCommitEventNotifier.SendExceptionMessageSafe(e, repositoryLogger);
        }
    }

    protected override async Task ProcessPullRequestReviewWebhookAsync(
        WebhookHeaders headers,
        PullRequestReviewEvent pullRequestReviewEvent,
        PullRequestReviewAction action)
    {
        GithubPullRequestDescriptor githubPullRequestDescriptor = CreateDescriptor(pullRequestReviewEvent);
        ILogger repositoryLogger = _logger.ToRepositoryLogger(githubPullRequestDescriptor);

        const string methodName = nameof(ProcessPullRequestReviewWebhookAsync);

        if (IsSenderBotOrNull(pullRequestReviewEvent))
        {
            repositoryLogger.LogTrace($"{methodName} was skipped because sender is bot or null");
            return;
        }

        string pullRequestReviewAction = action;

        repositoryLogger.LogInformation(
            "{MethodName}: {Name} with type {Action}",
            methodName,
            pullRequestReviewEvent.GetType().Name,
            pullRequestReviewAction);

        var pullRequestEventNotifier = new PullRequestEventNotifier(
            _actionNotifier,
            pullRequestReviewEvent,
            pullRequestReviewEvent.PullRequest.Number,
            repositoryLogger);

        try
        {
            string pullRequestReviewAction1 = action;

            switch (pullRequestReviewAction1)
            {
                case PullRequestReviewActionValue.Submitted when pullRequestReviewEvent.Review.State == "approved":
                    await _handler.ProcessPullRequestReviewApprove(
                        pullRequestReviewEvent.Review.Body,
                        githubPullRequestDescriptor,
                        repositoryLogger,
                        pullRequestEventNotifier,
                        default);
                    break;

                case PullRequestReviewActionValue.Submitted
                    when pullRequestReviewEvent.Review.State == "changes_requested":
                    await _handler.ProcessPullRequestReviewRequestChanges(
                        pullRequestReviewEvent.Review.Body,
                        githubPullRequestDescriptor,
                        repositoryLogger,
                        pullRequestEventNotifier,
                        default);
                    break;

                case PullRequestReviewActionValue.Submitted when pullRequestReviewEvent.Review.State == "commented":
                    await _handler.ProcessPullRequestReviewComment(
                        pullRequestReviewEvent.Review.Body,
                        githubPullRequestDescriptor,
                        repositoryLogger,
                        pullRequestEventNotifier,
                        default);
                    break;

                case PullRequestReviewActionValue.Edited:
                case PullRequestReviewActionValue.Dismissed:
                    repositoryLogger.LogWarning(
                        "Pull request review action {Action} is not supported",
                        pullRequestReviewAction1);
                    break;
                default:
                    repositoryLogger.LogWarning(
                        "Pull request review for pr {PrLink} is not processed",
                        githubPullRequestDescriptor.Payload);
                    break;
            }
        }
        catch (Exception e)
        {
            string message = $"Failed to handle {pullRequestReviewAction}";
            repositoryLogger.LogError(e, "{MethodName}:{Message}", methodName, message);

            await pullRequestEventNotifier.SendExceptionMessageSafe(e, repositoryLogger);
        }
    }

    protected override async Task ProcessIssueCommentWebhookAsync(
        WebhookHeaders headers,
        IssueCommentEvent issueCommentEvent,
        IssueCommentAction action)
    {
        GithubPullRequestDescriptor githubPullRequestDescriptor = await GetPullRequestDescriptor(issueCommentEvent);
        ILogger repositoryLogger = _logger.ToRepositoryLogger(githubPullRequestDescriptor);

        const string methodName = nameof(ProcessIssueCommentWebhookAsync);

        if (IsSenderBotOrNull(issueCommentEvent))
        {
            repositoryLogger.LogTrace($"{methodName} was skipped because sender is bot or null");
            return;
        }

        if (IsPullRequestCommand(issueCommentEvent) is false)
        {
            repositoryLogger.LogTrace(
                "Skipping commit in {IssueId}. Issue comments is not supported",
                issueCommentEvent.Issue.Id);

            return;
        }

        string issueCommentAction = action;

        repositoryLogger.LogInformation(
            "{MethodName}: {EventName} with type {Action}",
            methodName,
            issueCommentEvent.GetType().Name,
            issueCommentAction);

        var pullRequestCommentEventNotifier = new PullRequestCommentEventNotifier(
            _actionNotifier,
            issueCommentEvent,
            issueCommentEvent.Comment.Id,
            issueCommentEvent.Issue.Number,
            repositoryLogger);

        try
        {
            switch (issueCommentAction)
            {
                case IssueCommentActionValue.Created:
                    await _handler.ProcessIssueCommentCreate(
                        issueCommentEvent.Comment.Body,
                        githubPullRequestDescriptor,
                        repositoryLogger,
                        pullRequestCommentEventNotifier,
                        default);
                    break;

                case IssueCommentActionValue.Deleted:
                case IssueCommentActionValue.Edited:
                    repositoryLogger.LogTrace(
                        "Pull request comment {Action} event will be ignored",
                        issueCommentAction);
                    break;
            }
        }
        catch (Exception e)
        {
            string message = $"Failed to handle {issueCommentAction}";
            repositoryLogger.LogError(e, "{MethodName}: {Message}", methodName, message);

            await pullRequestCommentEventNotifier.SendExceptionMessageSafe(e, repositoryLogger);
        }
    }

    private bool IsPullRequestCommand(IssueCommentEvent issueCommentEvent)
    {
        return issueCommentEvent.Issue.PullRequest.Url is not null;
    }

    private bool IsSenderBotOrNull(WebhookEvent webhookEvent)
    {
        return webhookEvent.Sender is null || webhookEvent.Sender.Type == UserType.Bot;
    }

    private GithubPullRequestDescriptor CreateDescriptor(PullRequestEvent @event)
    {
        string login = @event.Sender!.Login;
        string payload = @event.PullRequest.HtmlUrl;
        string organization = @event.Organization!.Login;
        string repository = @event.Repository!.Name;
        string branch = @event.PullRequest.Head.Ref;
        long prNum = @event.PullRequest.Number;

        var pullRequestDescriptor = new GithubPullRequestDescriptor(
            login,
            payload,
            organization,
            repository,
            branch,
            prNum);

        return pullRequestDescriptor;
    }

    private GithubPullRequestDescriptor CreateDescriptor(PullRequestReviewEvent pullRequestReviewEvent)
    {
        return new GithubPullRequestDescriptor(
            pullRequestReviewEvent.Sender!.Login,
            pullRequestReviewEvent.Review.HtmlUrl,
            pullRequestReviewEvent.Organization!.Login,
            pullRequestReviewEvent.Repository!.Name,
            pullRequestReviewEvent.PullRequest.Head.Ref,
            pullRequestReviewEvent.PullRequest.Number);
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
            pullRequest.HtmlUrl,
            issueCommentEvent.Organization.Login,
            issueCommentEvent.Repository.Name,
            pullRequest.Head.Ref,
            pullRequest.Number);
    }
}