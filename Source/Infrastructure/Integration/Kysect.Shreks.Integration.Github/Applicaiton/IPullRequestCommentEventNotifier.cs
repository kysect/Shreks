namespace Kysect.Shreks.Integration.Github.Applicaiton;

public interface IPullRequestCommentEventNotifier : IPullRequestEventNotifier
{
    Task ReactToUserComment(bool isSuccess);
}