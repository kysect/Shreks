using Octokit.Webhooks;

namespace Kysect.Shreks.Integration.Github.Core.Entities;

public interface IActionNotifier
{ 
    Task ApplyInComments(WebhookEvent webhookEvent, long issueNumber, string hook);
}