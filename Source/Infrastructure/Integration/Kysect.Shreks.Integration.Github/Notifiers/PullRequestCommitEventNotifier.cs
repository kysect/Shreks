using Kysect.Shreks.Integration.Github.Applicaiton;
using Microsoft.Extensions.Logging;
using Octokit.Webhooks;

namespace Kysect.Shreks.Integration.Github.Notifiers;

public class PullRequestCommitEventNotifier : IPullRequestCommitEventNotifier
{
    private readonly ILogger _logger;
    private readonly IActionNotifier _actionNotifier;
    private readonly WebhookEvent _webhookEvent;
    private readonly string _comminSha;
    private readonly long _issueNumber;

    public PullRequestCommitEventNotifier(IActionNotifier actionNotifier, WebhookEvent webhookEvent, string comminSha, long issueNumber, ILogger logger)
    {
        _actionNotifier = actionNotifier;
        _webhookEvent = webhookEvent;
        _comminSha = comminSha;
        _issueNumber = issueNumber;
        _logger = logger;
    }

    public async Task SendCommentToTriggeredCommit(string message)
    {
        await _actionNotifier.SendCommitComment(_webhookEvent, _comminSha, message);
        _logger.LogInformation("Send comment to PR: " + message);
    }

    public async Task SendCommentToPullRequest(string message)
    {
        await _actionNotifier.SendComment(_webhookEvent, _issueNumber, message);
        _logger.LogInformation("Send comment to PR: " + message);
    }
}