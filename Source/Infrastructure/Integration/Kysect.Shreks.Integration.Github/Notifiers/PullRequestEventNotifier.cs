using Kysect.Shreks.Integration.Github.Applicaiton;
using Microsoft.Extensions.Logging;
using Octokit.Webhooks;

namespace Kysect.Shreks.Integration.Github.Notifiers;

public class PullRequestEventNotifier : IPullRequetsEventNotifier
{
    private readonly ILogger _logger;
    private readonly IActionNotifier _actionNotifier;
    private readonly WebhookEvent _webhookEvent;
    private readonly long _issueNumber;

    public PullRequestEventNotifier(IActionNotifier actionNotifier, WebhookEvent webhookEvent, long issueNumber, ILogger logger)
    {
        _actionNotifier = actionNotifier;
        _webhookEvent = webhookEvent;
        _issueNumber = issueNumber;
        _logger = logger;
    }

    public async Task SendCommentToPullRequest(string message)
    {
        if (string.IsNullOrEmpty(message))
            return;

        await _actionNotifier.SendComment(_webhookEvent, _issueNumber, message);
        _logger.LogInformation("Send comment to PR: " + message);
    }
}