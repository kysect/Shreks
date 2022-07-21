using Octokit.Webhooks;

namespace Kysect.Shreks.GithubIntegration.Entities;

public interface IActionNotifier
{ 
    Task ApplyInComments(WebhookEvent webhookEvent, long issueNumber, string hook);
}