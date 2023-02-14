namespace ITMO.Dev.ASAP.Application.GithubWorkflow.Abstractions.Notifications;

public interface IPullRequestCommitEventNotifier : IPullRequestEventNotifier
{
    Task SendCommentToTriggeredCommit(string message);
}