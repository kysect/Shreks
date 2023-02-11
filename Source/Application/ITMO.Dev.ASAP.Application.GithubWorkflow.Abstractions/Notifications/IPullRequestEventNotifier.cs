namespace ITMO.Dev.ASAP.Application.GithubWorkflow.Abstractions.Notifications;

public interface IPullRequestEventNotifier
{
    Task SendCommentToPullRequest(string message);
}