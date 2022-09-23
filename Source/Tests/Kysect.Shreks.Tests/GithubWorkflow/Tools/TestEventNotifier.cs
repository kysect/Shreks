using Kysect.Shreks.Application.GithubWorkflow.Abstractions;

namespace Kysect.Shreks.Tests.GithubWorkflow.Tools;

public class TestEventNotifier : IPullRequestEventNotifier
{
    public List<string> Messages { get; } = new List<string>();

    public Task SendCommentToPullRequest(string message)
    {
        Messages.Add(message);
        return Task.CompletedTask;
    }
}