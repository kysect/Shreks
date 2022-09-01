using Octokit.Webhooks;

namespace Kysect.Shreks.Integration.Github.Entities;

public interface IActionNotifier
{ 
    Task SendComment(WebhookEvent webhookEvent, long issueNumber, string message);
    Task SendCommitComment(WebhookEvent webhookEvent, string sha, string message);
    Task ReactInComments(WebhookEvent webhookEvent, long commentId, bool isSuccessful);
}