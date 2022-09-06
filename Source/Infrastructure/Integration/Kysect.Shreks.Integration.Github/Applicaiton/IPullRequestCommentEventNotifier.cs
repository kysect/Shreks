namespace Kysect.Shreks.Integration.Github.Applicaiton;

public interface IPullRequestCommentEventNotifier : IPullRequetsEventNotifier
{
    Task ReactToUserComment(bool isSuccess);
}