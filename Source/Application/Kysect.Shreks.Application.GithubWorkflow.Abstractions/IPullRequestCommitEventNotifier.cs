namespace Kysect.Shreks.Application.GithubWorkflow.Abstractions;

public interface IPullRequestCommitEventNotifier : IPullRequestEventNotifier
{
    Task SendCommentToTriggeredCommit(string message);
}