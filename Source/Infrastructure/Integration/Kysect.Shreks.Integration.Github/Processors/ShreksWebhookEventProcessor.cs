using Kysect.Shreks.Application.Abstractions.DataAccess;
using Kysect.Shreks.Application.Commands.Commands;
using Kysect.Shreks.Integration.Github.ContextCreators;
using Kysect.Shreks.Integration.Github.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Octokit.Webhooks;
using Octokit.Webhooks.Events;
using Octokit.Webhooks.Events.IssueComment;
using Octokit.Webhooks.Events.PullRequest;
using Octokit.Webhooks.Events.PullRequestReview;
using Octokit.Webhooks.Models;

namespace Kysect.Shreks.Integration.Github.Processors;

public sealed class ShreksWebhookEventProcessor : WebhookEventProcessor
{
    private readonly IActionNotifier _actionNotifier;
    private readonly ILogger<ShreksWebhookEventProcessor> _logger;
    private readonly IShreksCommandParser _commandParser;
    private readonly GithubCommandProcessor _commandProcessor;
    private readonly IMediator _mediator;
    private readonly IShreksDatabaseContext _databaseContext;

    public ShreksWebhookEventProcessor(
        IActionNotifier actionNotifier, 
        ILogger<ShreksWebhookEventProcessor> logger,
        GithubCommandProcessor commandProcessor, 
        IShreksCommandParser commandParser, 
        IMediator mediator, 
        IShreksDatabaseContext databaseContext)
    {
        _actionNotifier = actionNotifier;
        _logger = logger;
        _commandProcessor = commandProcessor;
        _commandParser = commandParser;
        _mediator = mediator;
        _databaseContext = databaseContext;
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
                //here we should update time and payload
                break;
            case PullRequestActionValue.Reopened:
            case PullRequestActionValue.Opened:
                break;
        }

        await _actionNotifier.ApplyInComments(
            pullRequestEvent,
            pullRequestEvent.PullRequest.Number,
            nameof(ProcessPullRequestWebhookAsync));
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

        switch (action)
        {
            case IssueCommentActionValue.Edited:
                break;
            case IssueCommentActionValue.Created:
                try
                {
                    IShreksCommand? command = _commandParser.Parse(issueCommentEvent.Comment.Body);
                    if (command != null)
                    {
                        var result = await command.Process(_commandProcessor,
                            new IssueCommentContextFactory(_mediator, issueCommentEvent, _databaseContext));
                        await _actionNotifier.SendComment(
                            issueCommentEvent,
                            issueCommentEvent.Issue.Number,
                            result.Message);

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

    private bool IsSenderBotOrNull(WebhookEvent webhookEvent) =>
        webhookEvent.Sender is null || webhookEvent.Sender.Type == UserType.Bot;
}