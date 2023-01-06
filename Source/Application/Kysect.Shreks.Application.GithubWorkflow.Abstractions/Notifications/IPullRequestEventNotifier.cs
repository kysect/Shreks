namespace Kysect.Shreks.Application.GithubWorkflow.Abstractions.Notifications;

public interface IPullRequestEventNotifier
{
    Task SendCommentToPullRequest(string message);
}