using Kysect.Shreks.Application.GithubWorkflow.Abstractions;
using Microsoft.Extensions.Logging;
using Octokit.Webhooks;

namespace Kysect.Shreks.Integration.Github.Notifiers;

public class PullRequestCommitEventNotifier : PullRequestEventNotifier, IPullRequestCommitEventNotifier
{
    private readonly ILogger _logger;
    private readonly IActionNotifier _actionNotifier;
    private readonly WebhookEvent _webhookEvent;
    private readonly string _commitSha;
    private readonly long _issueNumber;

    public PullRequestCommitEventNotifier(IActionNotifier actionNotifier, WebhookEvent webhookEvent, string commitSha, long issueNumber, ILogger logger)
        : base(actionNotifier, webhookEvent, issueNumber, logger)
    {
        _actionNotifier = actionNotifier;
        _webhookEvent = webhookEvent;
        _commitSha = commitSha;
        _issueNumber = issueNumber;
        _logger = logger;
    }

    public async Task SendCommentToTriggeredCommit(string message)
    {
        await _actionNotifier.SendCommitComment(_webhookEvent, _commitSha, message);
        _logger.LogInformation("Send comment to PR: " + message);
    }
}