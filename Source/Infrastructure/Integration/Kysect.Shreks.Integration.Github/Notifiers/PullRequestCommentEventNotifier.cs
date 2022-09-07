using Kysect.Shreks.Application.GithubWorkflow.Abstractions;
using Microsoft.Extensions.Logging;
using Octokit.Webhooks;

namespace Kysect.Shreks.Integration.Github.Notifiers;

public class PullRequestCommentEventNotifier : PullRequestEventNotifier, IPullRequestCommentEventNotifier
{
    private readonly ILogger _logger;
    private readonly IActionNotifier _actionNotifier;
    private readonly WebhookEvent _webhookEvent;
    private readonly long _commentId;
    private readonly long _issueNumber;

    public PullRequestCommentEventNotifier(IActionNotifier actionNotifier, WebhookEvent webhookEvent, long commentId, long issueNumber, ILogger logger)
        : base(actionNotifier, webhookEvent, issueNumber, logger)
    {
        _actionNotifier = actionNotifier;
        _webhookEvent = webhookEvent;
        _commentId = commentId;
        _issueNumber = issueNumber;
        _logger = logger;
    }

    public async Task ReactToUserComment(bool isSuccess)
    {
        await _actionNotifier.ReactInComments(
            _webhookEvent,
            _commentId,
            isSuccess);
        _logger.LogInformation("Send reaction: " + isSuccess);
    }
}