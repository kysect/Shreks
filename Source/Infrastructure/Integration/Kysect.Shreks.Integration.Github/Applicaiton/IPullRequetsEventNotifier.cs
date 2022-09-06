namespace Kysect.Shreks.Integration.Github.Applicaiton;

public interface IPullRequetsEventNotifier
{
    Task SendCommentToPullRequest(string message);
}