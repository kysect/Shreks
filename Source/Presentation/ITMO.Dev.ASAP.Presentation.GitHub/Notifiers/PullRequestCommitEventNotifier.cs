using ITMO.Dev.ASAP.Application.GithubWorkflow.Abstractions.Notifications;
using Microsoft.Extensions.Logging;
using Octokit.Webhooks;

namespace ITMO.Dev.ASAP.Presentation.GitHub.Notifiers;

public class PullRequestCommitEventNotifier : PullRequestEventNotifier, IPullRequestCommitEventNotifier
{
    private readonly IActionNotifier _actionNotifier;

    private readonly string _commitSha;

    /* private readonly long _issueNumber; */
    private readonly ILogger _logger;
    private readonly WebhookEvent _webhookEvent;

    public PullRequestCommitEventNotifier(
        IActionNotifier actionNotifier,
        WebhookEvent webhookEvent,
        string commitSha,
        long issueNumber,
        ILogger logger)
        : base(actionNotifier, webhookEvent, issueNumber, logger)
    {
        _actionNotifier = actionNotifier;
        _webhookEvent = webhookEvent;
        _commitSha = commitSha;
        /* _issueNumber = issueNumber; */
        _logger = logger;
    }

    public async Task SendCommentToTriggeredCommit(string message)
    {
        await _actionNotifier.SendCommitComment(_webhookEvent, _commitSha, message);
        _logger.LogInformation("Send comment to PR: {Message}", message);
    }
}