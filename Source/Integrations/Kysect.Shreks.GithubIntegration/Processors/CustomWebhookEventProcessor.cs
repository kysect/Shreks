using Kysect.Shreks.GithubIntegration.Client;
using Microsoft.Extensions.Logging;
using Octokit;
using Octokit.Webhooks;
using Octokit.Webhooks.Events;
using Octokit.Webhooks.Events.IssueComment;
using Octokit.Webhooks.Events.PullRequest;
using Octokit.Webhooks.Events.PullRequestReview;
using Octokit.Webhooks.Models;
using PullRequestReviewEvent = Octokit.Webhooks.Events.PullRequestReviewEvent;

namespace Kysect.Shreks.GithubIntegration.Processors;

public sealed class CustomWebhookEventProcessor : WebhookEventProcessor
{
    private readonly IInstallationClientFactory _installationClientFactory;
    private readonly ILogger<CustomWebhookEventProcessor> _logger;

    public CustomWebhookEventProcessor(IInstallationClientFactory installationClientFactory, ILogger<CustomWebhookEventProcessor> logger)
    {
        _installationClientFactory = installationClientFactory;
        _logger = logger;
    }

    protected override async Task ProcessPullRequestWebhookAsync(WebhookHeaders headers,
        PullRequestEvent pullRequestEvent, PullRequestAction action)
    {
        _logger.LogDebug($"{nameof(ProcessPullRequestWebhookAsync)}: {pullRequestEvent.GetType().Name}");

        if (IsSenderBot(pullRequestEvent))
        {
            _logger.LogTrace($"{nameof(ProcessPullRequestWebhookAsync)} skipped because sender is bot");
            return;
        }

        var installationClient = await _installationClientFactory.GetClient(pullRequestEvent.Installation.Id);

        switch (action)
        {
            case PullRequestActionValue.Synchronize:
                break;
            case PullRequestActionValue.Opened:
                break;
        }

        await installationClient.Issue.Comment.Create(
            pullRequestEvent.Repository.Owner.Login,
            pullRequestEvent.Repository.Name,
            (int) pullRequestEvent.PullRequest.Number, 
            $"**Hook**: `{nameof(ProcessPullRequestWebhookAsync)}`" + $"**Action**: `{pullRequestEvent.Action}`\n");
    }

    protected override async Task ProcessPullRequestReviewWebhookAsync(WebhookHeaders headers,
        PullRequestReviewEvent pullRequestReviewEvent, PullRequestReviewAction action)
    {
        _logger.LogDebug($"{nameof(ProcessPullRequestReviewWebhookAsync)}: {pullRequestReviewEvent.GetType().Name}");

        if (IsSenderBot(pullRequestReviewEvent))
        {
            _logger.LogTrace($"{nameof(ProcessPullRequestReviewWebhookAsync)} skipped because sender is bot");
            return;
        }

        var installationClient = await _installationClientFactory.GetClient(pullRequestReviewEvent.Installation.Id);

        switch (action)
        {
            case PullRequestReviewActionValue.Submitted:
                break;
            case PullRequestReviewActionValue.Edited:
                break;
            case PullRequestReviewActionValue.Dismissed:
                break;
        }

        await installationClient.Issue.Comment.Create(
            pullRequestReviewEvent.Repository.Owner.Login,
            pullRequestReviewEvent.Repository.Name,
            (int) pullRequestReviewEvent.PullRequest.Number,
            $"**Hook**: `{nameof(ProcessPullRequestWebhookAsync)}`\n" + $"**Action**: `{pullRequestReviewEvent.Action}`\n" + $"**Content**: `{pullRequestReviewEvent.Review.Body}`");
    }

    // PR comments
    protected override async Task ProcessIssueCommentWebhookAsync(WebhookHeaders headers,
        IssueCommentEvent issueCommentEvent, IssueCommentAction action)
    {
        _logger.LogDebug($"{nameof(ProcessIssueCommentWebhookAsync)}: {issueCommentEvent.GetType().Name}");

        if (IsSenderBot(issueCommentEvent))
        {
            _logger.LogTrace($"{nameof(ProcessIssueCommentWebhookAsync)} skipped because sender is bot");
            return;
        }

        var installationClient = await _installationClientFactory.GetClient(issueCommentEvent.Installation.Id);

        switch (action)
        {
            case IssueCommentActionValue.Edited:
                break;
            case IssueCommentActionValue.Created:
                break;
            case IssueCommentActionValue.Deleted:
                break;
        }

        await installationClient.Issue.Comment.Create(
            issueCommentEvent.Repository.Owner.Login,
            issueCommentEvent.Repository.Name,
            (int) issueCommentEvent.Issue.Number, 
            $"**Hook**: `{nameof(ProcessIssueCommentWebhookAsync)}`\n" + $"**Action**: `{issueCommentEvent.Action}`\n" + $"**Content**: `{issueCommentEvent.Comment.Body}`");
    }

    private bool IsSenderBot(WebhookEvent webhookEvent) => webhookEvent.Sender?.Type == UserType.Bot;
}