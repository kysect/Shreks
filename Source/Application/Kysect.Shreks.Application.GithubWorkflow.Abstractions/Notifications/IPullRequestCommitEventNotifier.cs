namespace Kysect.Shreks.Application.GithubWorkflow.Abstractions.Notifications;

public interface IPullRequestCommitEventNotifier : IPullRequestEventNotifier
{
    Task SendCommentToTriggeredCommit(string message);
}