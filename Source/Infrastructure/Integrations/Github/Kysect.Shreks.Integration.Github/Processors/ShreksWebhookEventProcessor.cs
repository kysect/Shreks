using Kysect.Shreks.Integration.Github.Core.Entities;
using Kysect.Shreks.Integration.Github.Core.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
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

    public ShreksWebhookEventProcessor(IActionNotifier actionNotifier, ILogger<ShreksWebhookEventProcessor> logger)
    {
        _actionNotifier = actionNotifier;
        _logger = logger;
    }

    protected override async Task ProcessPullRequestWebhookAsync(WebhookHeaders headers,
        PullRequestEvent pullRequestEvent, PullRequestAction action)
    {
        _logger.LogDebug($"{nameof(ProcessPullRequestWebhookAsync)}: {pullRequestEvent.GetType().Name}");

        var sender = GetSender(pullRequestEvent);

        if (IsSenderBot(sender))
        {
            _logger.LogTrace($"{nameof(ProcessPullRequestWebhookAsync)} skipped because sender is bot or null");
            return;
        }

        switch (action)
        {
            case PullRequestActionValue.Synchronize:
                break;
            case PullRequestActionValue.Opened:
                break;
        }

        await _actionNotifier.ApplyInComments(
            pullRequestEvent, 
            (int) pullRequestEvent.PullRequest.Number,
            nameof(ProcessPullRequestWebhookAsync));
    }

    protected override async Task ProcessPullRequestReviewWebhookAsync(WebhookHeaders headers,
        PullRequestReviewEvent pullRequestReviewEvent, PullRequestReviewAction action)
    {
        _logger.LogDebug($"{nameof(ProcessPullRequestReviewWebhookAsync)}: {pullRequestReviewEvent.GetType().Name}");

        var sender = GetSender(pullRequestReviewEvent);

        if (IsSenderBot(sender))
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
            (int) pullRequestReviewEvent.PullRequest.Number,
            nameof(ProcessPullRequestWebhookAsync));
    }

    protected override async Task ProcessIssueCommentWebhookAsync(WebhookHeaders headers,
        IssueCommentEvent issueCommentEvent, IssueCommentAction action)
    {
        _logger.LogDebug($"{nameof(ProcessIssueCommentWebhookAsync)}: {issueCommentEvent.GetType().Name}");

        var sender = GetSender(issueCommentEvent);

        if (IsSenderBot(sender))
        {
            _logger.LogTrace($"{nameof(ProcessIssueCommentWebhookAsync)} skipped because sender is bot or null");
            return;
        }

        switch (action)
        {
            case IssueCommentActionValue.Edited:
                break;
            case IssueCommentActionValue.Created:
                break;
            case IssueCommentActionValue.Deleted:
                break;
        }

        await _actionNotifier.ApplyInComments(
            issueCommentEvent,
            (int) issueCommentEvent.Issue.Number,
            nameof(ProcessIssueCommentWebhookAsync));
    }

    private bool IsSenderBot(User sender) => sender.Type == UserType.Bot;

    private User GetSender(WebhookEvent webhookEvent)
    {
        var sender =  webhookEvent.Sender;

        if (sender is not null) return sender;

        _logger.LogError($"Sender for webhook event {webhookEvent.Action} is null");
        throw new SenderNotFoundException($"Sender for webhook event {webhookEvent.Action} is null");
    }
}