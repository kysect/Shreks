namespace Kysect.Shreks.Integration.Github.Applicaiton;

public interface IPullRequestEventNotifier
{
    Task SendCommentToPullRequest(string message);
}