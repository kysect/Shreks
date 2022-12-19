using Kysect.Shreks.Application.GithubWorkflow.Abstractions;
using Kysect.Shreks.Application.GithubWorkflow.Abstractions.Models;
using Kysect.Shreks.Integration.Github.Client;
using Kysect.Shreks.Integration.Github.Helpers;
using Kysect.Shreks.Integration.Github.Notifiers;
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

public sealed class ShreksWebhookEventProcessorProxy : WebhookEventProcessor
{
    private readonly IActionNotifier _actionNotifier;
    private readonly ILogger<ShreksWebhookEventProcessorProxy> _logger;
    private readonly IOrganizationGithubClientProvider _clientProvider;
    private readonly IShreksWebhookEventProcessor _processor;

    public ShreksWebhookEventProcessorProxy(
        IActionNotifier actionNotifier,
        ILogger<ShreksWebhookEventProcessorProxy> logger,
        IOrganizationGithubClientProvider clientProvider,
        IShreksWebhookEventProcessor eventProcessor)
    {
        _actionNotifier = actionNotifier;
        _logger = logger;
        _clientProvider = clientProvider;
        _processor = eventProcessor;
    }

    protected override async Task ProcessPullRequestWebhookAsync(
        WebhookHeaders headers,
        PullRequestEvent pullRequestEvent,
        PullRequestAction action)
    {
        GithubPullRequestDescriptor githubPullRequestDescriptor = CreateDescriptor(pullRequestEvent);
        ILogger repositoryLogger = _logger.ToRepositoryLogger(githubPullRequestDescriptor);

        if (IsSenderBotOrNull(pullRequestEvent))
        {
            repositoryLogger.LogTrace($"{nameof(ProcessPullRequestWebhookAsync)} was skipped because sender is bot or null");
            return;
        }

        string pullRequestAction = action;
        repositoryLogger.LogInformation($"{nameof(ProcessPullRequestWebhookAsync)}: {pullRequestEvent.GetType().Name} with type {pullRequestAction}");
        var pullRequestCommitEventNotifier = new PullRequestCommitEventNotifier(_actionNotifier, pullRequestEvent, pullRequestEvent.PullRequest.Head.Sha, pullRequestEvent.PullRequest.Number, repositoryLogger);

        try
        {
            CancellationToken cancellationToken = CancellationToken.None;

            switch (pullRequestAction)
            {
                case PullRequestActionValue.Synchronize:
                case PullRequestActionValue.Opened:
                    await _processor.ProcessPullRequestUpdate(githubPullRequestDescriptor, repositoryLogger, pullRequestCommitEventNotifier, cancellationToken);
                    break;

                case PullRequestActionValue.Reopened:
                    await _processor.ProcessPullRequestReopen(githubPullRequestDescriptor, repositoryLogger, pullRequestCommitEventNotifier);
                    break;

                case PullRequestActionValue.Closed:
                    bool merged = pullRequestEvent.PullRequest.Merged ?? false;
                    await _processor.ProcessPullRequestClosed(merged, githubPullRequestDescriptor, repositoryLogger, pullRequestCommitEventNotifier);
                    break;

                case PullRequestActionValue.Assigned:
                case PullRequestActionValue.ReviewRequestRemoved:
                case PullRequestActionValue.ReviewRequested:
                    repositoryLogger.LogDebug($"Skip pull request action with type {pullRequestAction}.");
                    break;

                default:
                    repositoryLogger.LogWarning($"Unsupported pull request webhook type was received: {pullRequestAction}");
                    break;
            }
        }
        catch (Exception e)
        {
            string message = $"Failed to handle {pullRequestAction}";
            repositoryLogger.LogError(e, "{MethodName}: {Message}", nameof(ProcessPullRequestWebhookAsync), message);
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

        if (IsSenderBotOrNull(pullRequestReviewEvent))
        {
            repositoryLogger.LogTrace($"{nameof(ProcessPullRequestReviewWebhookAsync)} was skipped because sender is bot or null");
            return;
        }

        string pullRequestReviewAction = action;
        repositoryLogger.LogInformation($"{nameof(ProcessPullRequestReviewWebhookAsync)}: {pullRequestReviewEvent.GetType().Name} with type {pullRequestReviewAction}");
        var pullRequestEventNotifier = new PullRequestEventNotifier(_actionNotifier, pullRequestReviewEvent, pullRequestReviewEvent.PullRequest.Number, repositoryLogger);

        try
        {
            string pullRequestReviewAction1 = action;

            switch (pullRequestReviewAction1)
            {
                case PullRequestReviewActionValue.Submitted when pullRequestReviewEvent.Review.State == "approved":
                    await _processor.ProcessPullRequestReviewApprove(pullRequestReviewEvent.Review.Body, githubPullRequestDescriptor, repositoryLogger, pullRequestEventNotifier);
                    break;

                case PullRequestReviewActionValue.Submitted when pullRequestReviewEvent.Review.State == "changes_requested":
                    await _processor.ProcessPullRequestReviewRequestChanges(pullRequestReviewEvent.Review.Body, githubPullRequestDescriptor, repositoryLogger, pullRequestEventNotifier);
                    break;

                case PullRequestReviewActionValue.Submitted when pullRequestReviewEvent.Review.State == "commented":
                    await _processor.ProcessPullRequestReviewComment(pullRequestReviewEvent.Review.Body, githubPullRequestDescriptor, repositoryLogger, pullRequestEventNotifier);
                    break;

                case PullRequestReviewActionValue.Edited:
                case PullRequestReviewActionValue.Dismissed:

                    repositoryLogger.LogWarning($"Pull request review action {pullRequestReviewAction1} is not supported.");
                    break;
                default:
                    repositoryLogger.LogWarning("Pull request review for pr {prLink} is not processed.", githubPullRequestDescriptor.Payload);
                    break;
            }
        }
        catch (Exception e)
        {
            string message = $"Failed to handle {pullRequestReviewAction}";
            repositoryLogger.LogError(e, $"{nameof(ProcessPullRequestReviewWebhookAsync)}:{message}");
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

        if (IsSenderBotOrNull(issueCommentEvent))
        {
            repositoryLogger.LogTrace($"{nameof(ProcessIssueCommentWebhookAsync)} was skipped because sender is bot or null");
            return;
        }

        if (!IsPullRequestCommand(issueCommentEvent))
        {
            repositoryLogger.LogTrace($"Skipping commit in {issueCommentEvent.Issue.Id}. Issue comments is not supported.");
            return;
        }

        string issueCommentAction = action;
        repositoryLogger.LogInformation($"{nameof(ProcessIssueCommentWebhookAsync)}: {issueCommentEvent.GetType().Name} with type {issueCommentAction}");
        var pullRequestCommentEventNotifier = new PullRequestCommentEventNotifier(_actionNotifier, issueCommentEvent, issueCommentEvent.Comment.Id, issueCommentEvent.Issue.Number, repositoryLogger);

        try
        {
            string issueCommentAction1 = action;
            switch (issueCommentAction1)
            {
                case IssueCommentActionValue.Created:
                    await _processor.ProcessIssueCommentCreate(issueCommentEvent.Comment.Body, githubPullRequestDescriptor, repositoryLogger, pullRequestCommentEventNotifier);
                    break;

                case IssueCommentActionValue.Deleted:
                case IssueCommentActionValue.Edited:
                    repositoryLogger.LogTrace($"Pull request comment {issueCommentAction1} event will be ignored.");
                    break;
            }
        }
        catch (Exception e)
        {
            string message = $"Failed to handle {issueCommentAction}";
            repositoryLogger.LogError(e, $"{nameof(ProcessIssueCommentWebhookAsync)}: {message}");
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

    public GithubPullRequestDescriptor CreateDescriptor(PullRequestEvent @event)
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

    public GithubPullRequestDescriptor CreateDescriptor(PullRequestReviewEvent pullRequestReviewEvent)
    {
        return new GithubPullRequestDescriptor(
            pullRequestReviewEvent.Sender!.Login,
            Payload: pullRequestReviewEvent.Review.HtmlUrl,
            pullRequestReviewEvent.Organization!.Login,
            pullRequestReviewEvent.Repository!.Name,
            BranchName: pullRequestReviewEvent.PullRequest.Head.Ref,
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
            Payload: pullRequest.HtmlUrl,
            issueCommentEvent.Organization.Login,
            issueCommentEvent.Repository.Name,
            BranchName: pullRequest.Head.Ref,
            pullRequest.Number);
    }
}