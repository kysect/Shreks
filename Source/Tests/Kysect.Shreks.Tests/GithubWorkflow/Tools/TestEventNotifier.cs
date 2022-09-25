using Kysect.Shreks.Application.GithubWorkflow.Abstractions;

namespace Kysect.Shreks.Tests.GithubWorkflow.Tools;

public class TestEventNotifier : IPullRequestCommitEventNotifier
{
    public List<string> PullRequestMessages { get; } = new List<string>();
    public List<string> CommitMessages { get; } = new List<string>();

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