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
    private readonly GitHubClient _gitHubClient;
    private readonly ILogger<CustomWebhookEventProcessor> _logger;

    public CustomWebhookEventProcessor(GitHubClient gitHubClient, ILogger<CustomWebhookEventProcessor> logger)
    {
        _gitHubClient = gitHubClient;
        _logger = logger;
    }

    protected override async Task ProcessPullRequestWebhookAsync(WebhookHeaders headers,
        PullRequestEvent pullRequestEvent, PullRequestAction action)
    {
        _logger.Log(LogLevel.Debug, $"{nameof(ProcessPullRequestWebhookAsync)}: {pullRequestEvent.GetType().Name}");

        if (IsSenderBot(pullRequestEvent)) return;

        var installationClient = await GetAuthenticatedInstallationClient(pullRequestEvent);

        switch (pullRequestEvent.Action)
        {
            case "synchronize":
                break;
            case "opened":
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
        _logger.Log(LogLevel.Debug, $"{nameof(ProcessPullRequestReviewWebhookAsync)}: {pullRequestReviewEvent.GetType().Name}");

        if (IsSenderBot(pullRequestReviewEvent)) return;

        var installationClient = await GetAuthenticatedInstallationClient(pullRequestReviewEvent);

        switch (pullRequestReviewEvent.Action)
        {
            case "submitted":
                break;
            case "edited":
                break;
            case "dismissed":
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
        _logger.Log(LogLevel.Debug, $"{nameof(ProcessIssueCommentWebhookAsync)}: {issueCommentEvent.GetType().Name}");

        if (IsSenderBot(issueCommentEvent)) return;

        var installationClient = await GetAuthenticatedInstallationClient(issueCommentEvent);

        switch (issueCommentEvent.Action)
        {
            case "created":
                break;
            case "edited":
                break;
            case "deleted":
                break;
        }

        await installationClient.Issue.Comment.Create(
            issueCommentEvent.Repository.Owner.Login,
            issueCommentEvent.Repository.Name,
            (int) issueCommentEvent.Issue.Number, 
            $"**Hook**: `{nameof(ProcessIssueCommentWebhookAsync)}`\n" + $"**Action**: `{issueCommentEvent.Action}`\n" + $"**Content**: `{issueCommentEvent.Comment.Body}`");
    }

    private async Task<GitHubClient> GetAuthenticatedInstallationClient(WebhookEvent webhookEvent)
    {
        var accessToken = await CreateInstallationToken(webhookEvent);

        return new GitHubClient(new ProductHeaderValue($"Installation-{webhookEvent.Installation.Id}"))
        {
            Credentials = new Credentials(accessToken.Token)
        };
    }

    private async Task<AccessToken> CreateInstallationToken(WebhookEvent webhookEvent)
    {
        var installation = webhookEvent.Installation;
        return await _gitHubClient.GitHubApps.CreateInstallationToken(installation.Id);
    }

    private bool IsSenderBot(WebhookEvent webhookEvent) => webhookEvent.Sender?.Type == UserType.Bot;
}