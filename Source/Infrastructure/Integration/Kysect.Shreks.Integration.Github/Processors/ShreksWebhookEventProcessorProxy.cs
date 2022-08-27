using Kysect.Shreks.Application.Commands.Parsers;
using Kysect.Shreks.Integration.Github.Client;
using Kysect.Shreks.Integration.Github.Entities;
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

public sealed class ShreksWebhookEventProcessorProxy : WebhookEventProcessor
{
    private readonly IActionNotifier _actionNotifier;
    private readonly ILogger<ShreksWebhookEventProcessorProxy> _logger;
    private readonly ShreksWebhookEventProcessor _processor;

    public ShreksWebhookEventProcessorProxy(
        IActionNotifier actionNotifier,
        ILogger<ShreksWebhookEventProcessorProxy> logger,
        IShreksCommandParser commandParser,
        IMediator mediator,
        IOrganizationGithubClientProvider clientProvider)
    {
        _actionNotifier = actionNotifier;
        _logger = logger;

        _processor = new ShreksWebhookEventProcessor(
            actionNotifier,
            logger,
            commandParser,
            mediator,
            clientProvider);
    }

    protected override async Task ProcessPullRequestWebhookAsync(
        WebhookHeaders headers,
        PullRequestEvent pullRequestEvent,
        PullRequestAction action)
    {
        if (IsSenderBotOrNull(pullRequestEvent))
        {
            _logger.LogTrace($"{nameof(ProcessPullRequestWebhookAsync)} skipped because sender is bot or null");
            return;
        }

        _logger.LogInformation($"{nameof(ProcessPullRequestWebhookAsync)}: {pullRequestEvent.GetType().Name} with type {action}");

        try
        {
            await _processor.ProcessPullRequestWebhookAsync(headers, pullRequestEvent, action);
        }
        catch (Exception e)
        {
            string message = $"{nameof(ProcessPullRequestWebhookAsync)}: Failed to handle {action}";
            _logger.LogError(e, message);
            await SendExceptionMessageSafe(pullRequestEvent, (int)pullRequestEvent.PullRequest.Number, message);
        }
    }

    protected override async Task ProcessPullRequestReviewWebhookAsync(
        WebhookHeaders headers,
        PullRequestReviewEvent pullRequestReviewEvent,
        PullRequestReviewAction action)
    {
        if (IsSenderBotOrNull(pullRequestReviewEvent))
        {
            _logger.LogTrace($"{nameof(ProcessPullRequestReviewWebhookAsync)} skipped because sender is bot or null");
            return;
        }

        _logger.LogInformation($"{nameof(ProcessPullRequestReviewWebhookAsync)}: {pullRequestReviewEvent.GetType().Name}");

        try
        {
            await _processor.ProcessPullRequestReviewWebhookAsync(headers, pullRequestReviewEvent, action);
        }
        catch (Exception e)
        {
            string message = $"{nameof(ProcessPullRequestReviewWebhookAsync)}: Failed to handle {action}";
            _logger.LogError(e, message);
            await SendExceptionMessageSafe(pullRequestReviewEvent, (int)pullRequestReviewEvent.PullRequest.Number, message);
        }
    }

    protected override async Task ProcessIssueCommentWebhookAsync(
        WebhookHeaders headers,
        IssueCommentEvent issueCommentEvent,
        IssueCommentAction action)
    {
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

        _logger.LogInformation($"{nameof(ProcessIssueCommentWebhookAsync)}: {issueCommentEvent.GetType().Name} with type {action}");

        try
        {
            await _processor.ProcessIssueCommentWebhookAsync(headers, issueCommentEvent, action);
        }
        catch (Exception e)
        {
            string message = $"{nameof(ProcessIssueCommentWebhookAsync)}: Failed to handle {action}";
            _logger.LogError(e, message);
            await SendExceptionMessageSafe(issueCommentEvent, (int)issueCommentEvent.Issue.Number, message);
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

    private async Task SendExceptionMessageSafe(WebhookEvent webhookEvent, int issueNumber, string message)
    {
        try
        {
            await _actionNotifier.SendComment(webhookEvent, issueNumber, message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to send exception message to user pull request.");
        }
    }
}