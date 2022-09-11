namespace Kysect.Shreks.Application.GithubWorkflow.Abstractions;

public interface IPullRequestCommentEventNotifier : IPullRequestEventNotifier
{
    Task ReactToUserComment(bool isSuccess);
}