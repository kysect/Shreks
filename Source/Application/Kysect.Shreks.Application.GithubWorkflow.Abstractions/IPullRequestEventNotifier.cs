namespace Kysect.Shreks.Application.GithubWorkflow.Abstractions;

public interface IPullRequestEventNotifier
{
    Task SendCommentToPullRequest(string message);
}