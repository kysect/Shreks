using Octokit.Webhooks;

namespace Kysect.Shreks.Integration.Github.Entities;

public interface IActionNotifier
{ 
    Task ApplyInComments(WebhookEvent webhookEvent, long issueNumber, string hook);
    Task SendComment(WebhookEvent webhookEvent, long issueNumber, string message);
    Task ReactInComments(WebhookEvent webhookEvent, long commentId, bool isSuccessful);
}