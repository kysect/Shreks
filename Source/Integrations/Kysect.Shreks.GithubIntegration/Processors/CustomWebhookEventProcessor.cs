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
    private readonly GitHubClient _installationClient;
    private readonly ILogger<CustomWebhookEventProcessor> _logger;

    public CustomWebhookEventProcessor(GitHubClient installationClient, ILogger<CustomWebhookEventProcessor> logger)
    {
        _installationClient = installationClient;
        logger.LogError(installationClient.GitHubApps.GetAllInstallationsForCurrent().ToString());
        _logger = logger;
    }

    protected override async Task ProcessPullRequestWebhookAsync(WebhookHeaders headers,
        PullRequestEvent pullRequestEvent, PullRequestAction action)
    {
        _logger.Log(LogLevel.Debug, $"{nameof(ProcessPullRequestWebhookAsync)}: {pullRequestEvent.GetType().Name}");

        if (IsSenderBot(pullRequestEvent)) return;

        switch (action)
        {
            case PullRequestActionValue.Synchronize:
                break;
            case PullRequestActionValue.Opened:
                break;
        }

        await _installationClient.Issue.Comment.Create(
            pullRequestEvent.Repository.Owner.Login,
            pullRequestEvent.Repository.Name,
            (int) pullRequestEvent.PullRequest.Number, 
            $"**Hook**: `{nameof(ProcessPullRequestWebhookAsync)}`" + $"**Action**: `{pullRequestEvent.Action}`\n");
    }

    protected override async Task ProcessPullRequestReviewWebhookAsync(WebhookHeaders headers,
        PullRequestReviewEvent pullRequestReviewEvent, PullRequestReviewAction action)
    {
        _logger.Log(LogLevel.Debug, $"{nameof(ProcessPullRequestReviewWebhookAsync)}: {pullRequestReviewEvent.GetType().Name}");

        if (IsSenderBot(pullRequestReviewEvent)) return;

        switch (action)
        {
            case PullRequestReviewActionValue.Submitted:
                break;
            case PullRequestReviewActionValue.Edited:
                break;
            case PullRequestReviewActionValue.Dismissed:
                break;
        }

        await _installationClient.Issue.Comment.Create(
            pullRequestReviewEvent.Repository.Owner.Login,
            pullRequestReviewEvent.Repository.Name,
            (int) pullRequestReviewEvent.PullRequest.Number,
            $"**Hook**: `{nameof(ProcessPullRequestWebhookAsync)}`\n" + $"**Action**: `{pullRequestReviewEvent.Action}`\n" + $"**Content**: `{pullRequestReviewEvent.Review.Body}`");
    }

    // PR comments
    protected override async Task ProcessIssueCommentWebhookAsync(WebhookHeaders headers,
        IssueCommentEvent issueCommentEvent, IssueCommentAction action)
    {
        _logger.Log(LogLevel.Debug, $"{nameof(ProcessIssueCommentWebhookAsync)}: {issueCommentEvent.GetType().Name}");

        if (IsSenderBot(issueCommentEvent)) return;

        switch (action)
        {
            case IssueCommentActionValue.Edited:
                break;
            case IssueCommentActionValue.Created:
                break;
            case IssueCommentActionValue.Deleted:
                break;
        }

        await _installationClient.Issue.Comment.Create(
            issueCommentEvent.Repository.Owner.Login,
            issueCommentEvent.Repository.Name,
            (int) issueCommentEvent.Issue.Number, 
            $"**Hook**: `{nameof(ProcessIssueCommentWebhookAsync)}`\n" + $"**Action**: `{issueCommentEvent.Action}`\n" + $"**Content**: `{issueCommentEvent.Comment.Body}`");
    }

    private bool IsSenderBot(WebhookEvent webhookEvent) => webhookEvent.Sender?.Type == UserType.Bot;
}