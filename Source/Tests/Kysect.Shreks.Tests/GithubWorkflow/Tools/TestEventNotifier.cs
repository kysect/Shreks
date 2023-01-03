using Kysect.Shreks.Application.GithubWorkflow.Abstractions.Notifications;

namespace Kysect.Shreks.Tests.GithubWorkflow.Tools;

public class TestEventNotifier : IPullRequestCommitEventNotifier
{
    public ICollection<string> PullRequestMessages { get; } = Array.Empty<string>();

    public ICollection<string> CommitMessages { get; } = Array.Empty<string>();

    public Task SendCommentToPullRequest(string message)
    {
        PullRequestMessages.Add(message);
        return Task.CompletedTask;
    }

    public Task SendCommentToTriggeredCommit(string message)
    {
        CommitMessages.Add(message);
        return Task.CompletedTask;
    }
}