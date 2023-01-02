namespace Kysect.Shreks.Application.GithubWorkflow.Abstractions.Notifications;

public interface IPullRequestCommentEventNotifier : IPullRequestEventNotifier
{
    Task ReactToUserComment(bool isSuccess);
}