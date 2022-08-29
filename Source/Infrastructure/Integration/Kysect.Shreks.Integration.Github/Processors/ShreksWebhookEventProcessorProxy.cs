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
    private readonly ILogger<ShreksWebhookEventProcessorProxy> _logger;
    private readonly ShreksWebhookEventProcessor _processor;
    private readonly ShreksWebhookCommentSender _commentSender;

    public ShreksWebhookEventProcessorProxy(
        IActionNotifier actionNotifier,
        ILogger<ShreksWebhookEventProcessorProxy> logger,
        IShreksCommandParser commandParser,
        IMediator mediator,
        IOrganizationGithubClientProvider clientProvider)
    {
        _logger = logger;

        _commentSender = new ShreksWebhookCommentSender(actionNotifier, _logger);

        _processor = new ShreksWebhookEventProcessor(_commentSender, commandParser, mediator, clientProvider, logger);
    }

    protected override async Task ProcessPullRequestWebhookAsync(
        WebhookHeaders headers,
        PullRequestEvent pullRequestEvent,
        PullRequestAction action)
    {
        if (IsSenderBotOrNull(pullRequestEvent))
        {
            _logger.LogTrace($"{nameof(ProcessPullRequestWebhookAsync)} was skipped because sender is bot or null");
            return;
        }

        string pullRequestAction = action;
        _logger.LogInformation($"{nameof(ProcessPullRequestWebhookAsync)}: {pullRequestEvent.GetType().Name} with type {pullRequestAction}");

        try
        {
            await _processor.ProcessPullRequestWebhookAsync(headers, pullRequestEvent, action);
        }
        catch (Exception e)
        {

            string message = $"Failed to handle {pullRequestAction}";
            _logger.LogError(e, $"{nameof(ProcessPullRequestWebhookAsync)}: {message}");
            int issueNumber = (int)pullRequestEvent.PullRequest.Number;
            await _commentSender.SendExceptionMessageSafe(pullRequestEvent, issueNumber, e);
        }
    }

    protected override async Task ProcessPullRequestReviewWebhookAsync(
        WebhookHeaders headers,
        PullRequestReviewEvent pullRequestReviewEvent,
        PullRequestReviewAction action)
    {
        if (IsSenderBotOrNull(pullRequestReviewEvent))
        {
            _logger.LogTrace($"{nameof(ProcessPullRequestReviewWebhookAsync)} was skipped because sender is bot or null");
            return;
        }

        string pullRequestReviewAction = action;
        _logger.LogInformation($"{nameof(ProcessPullRequestReviewWebhookAsync)}: {pullRequestReviewEvent.GetType().Name} with type {pullRequestReviewAction}");

        try
        {
            await _processor.ProcessPullRequestReviewWebhookAsync(headers, pullRequestReviewEvent, action);
        }
        catch (Exception e)
        {
            string message = $"Failed to handle {pullRequestReviewAction}";
            _logger.LogError(e, $"{nameof(ProcessPullRequestReviewWebhookAsync)}:{message}");
            int issueNumber = (int)pullRequestReviewEvent.PullRequest.Number;
            await _commentSender.SendExceptionMessageSafe(pullRequestReviewEvent, issueNumber, e);
        }
    }

    protected override async Task ProcessIssueCommentWebhookAsync(
        WebhookHeaders headers,
        IssueCommentEvent issueCommentEvent,
        IssueCommentAction action)
    {
        if (IsSenderBotOrNull(issueCommentEvent))
        {
            _logger.LogTrace($"{nameof(ProcessIssueCommentWebhookAsync)} was skipped because sender is bot or null");
            return;
        }

        if (!IsPullRequestCommand(issueCommentEvent))
        {
            _logger.LogTrace($"Skipping commit in {issueCommentEvent.Issue.Id}. Issue comments is not supported.");
            return;
        }

        string issueCommentAction = action;
        _logger.LogInformation($"{nameof(ProcessIssueCommentWebhookAsync)}: {issueCommentEvent.GetType().Name} with type {issueCommentAction}");

        try
        {
            await _processor.ProcessIssueCommentWebhookAsync(headers, issueCommentEvent, action);
        }
        catch (Exception e)
        {
            string message = $"Failed to handle {issueCommentAction}";
            _logger.LogError(e, $"{nameof(ProcessIssueCommentWebhookAsync)}: {message}");
            int issueNumber = (int)issueCommentEvent.Issue.Number;
            await _commentSender.SendExceptionMessageSafe(issueCommentEvent, issueNumber, e);
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
}