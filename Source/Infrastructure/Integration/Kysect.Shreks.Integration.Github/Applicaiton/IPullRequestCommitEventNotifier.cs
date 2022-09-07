namespace Kysect.Shreks.Integration.Github.Applicaiton;

public interface IPullRequestCommitEventNotifier : IPullRequestEventNotifier
{
    Task SendCommentToTriggeredCommit(string message);
}