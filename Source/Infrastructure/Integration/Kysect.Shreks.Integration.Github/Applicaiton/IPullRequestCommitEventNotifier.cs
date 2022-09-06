namespace Kysect.Shreks.Integration.Github.Applicaiton;

public interface IPullRequestCommitEventNotifier : IPullRequetsEventNotifier
{
    Task SendCommentToTriggeredCommit(string message);
}