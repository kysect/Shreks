using Kysect.Shreks.Application.Abstractions.Submissions.Commands;
using Kysect.Shreks.Application.Commands.Commands;
using Kysect.Shreks.Core.Study;
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

public sealed class ShreksWebhookEventProcessor : WebhookEventProcessor
{
    private readonly IActionNotifier _actionNotifier;
    private readonly ILogger<ShreksWebhookEventProcessor> _logger;
    private readonly IShreksCommandParser _commandParser;
    private readonly GithubCommandProcessor _commandProcessor;
    private readonly IMediator _mediator;

    public ShreksWebhookEventProcessor(IActionNotifier actionNotifier, ILogger<ShreksWebhookEventProcessor> logger,
        GithubCommandProcessor commandProcessor, IShreksCommandParser commandParser, IMediator mediator)
    {
        _actionNotifier = actionNotifier;
        _logger = logger;
        _commandProcessor = commandProcessor;
        _commandParser = commandParser;
        _mediator = mediator;
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
            case PullRequestActionValue.Opened:
                var response = await _mediator.Send(new CreateSubmissionCommand.Command(
                    Guid.Empty, //testing only, will remove after merge of queries
                    Guid.Empty, 
                    pullRequestEvent.PullRequest.DiffUrl));
                await _actionNotifier.ApplyInComments(
                    pullRequestEvent,
                    pullRequestEvent.PullRequest.Number,
                    string.Format("Created submission with id {0}", response.SubmissionId));
                return;
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
                IShreksCommand? command = _commandParser.Parse(issueCommentEvent.Comment.Body);
                if (command != null)
                {
                    var result =  await command.Process(_commandProcessor, new ShreksCommandContext(null!, null)); //for testing, will remove after merge of queries
                    await _actionNotifier.ApplyInComments(
                        issueCommentEvent,
                        issueCommentEvent.Issue.Number,
                        result.Message);

                    await _actionNotifier.ReactInComments(
                        issueCommentEvent,
                        issueCommentEvent.Comment.Id,
                        result.IsSuccess);
                }
                break;
            case IssueCommentActionValue.Deleted:
                break;
        }

        await _actionNotifier.ApplyInComments(
            issueCommentEvent,
            issueCommentEvent.Issue.Number,
            nameof(ProcessIssueCommentWebhookAsync));

        await _actionNotifier.ReactInComments(
            issueCommentEvent,
            issueCommentEvent.Comment.Id,
            true);
    }

    private bool IsSenderBotOrNull(WebhookEvent webhookEvent) =>
        webhookEvent.Sender is null || webhookEvent.Sender.Type == UserType.Bot;
}