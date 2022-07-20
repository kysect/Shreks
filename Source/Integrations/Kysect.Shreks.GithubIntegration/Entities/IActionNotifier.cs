using Octokit.Webhooks;

namespace Kysect.Shreks.GithubIntegration.Entities;

public interface IActionNotifier
{
    public Task ApplyInComments(WebhookEvent webhookEvent, long issueNumber, string hook);
}