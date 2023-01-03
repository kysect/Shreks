using Octokit.Webhooks;

namespace Kysect.Shreks.Presentation.GitHub.Notifiers;

public interface IActionNotifier
{
    Task SendComment(WebhookEvent webhookEvent, long issueNumber, string message);

    Task SendCommitComment(WebhookEvent webhookEvent, string sha, string message);

    Task ReactInComments(WebhookEvent webhookEvent, long commentId, bool isSuccessful);
}