namespace Kysect.Shreks.Abstractions;

public record PullRequestCreateEventArguments();
public record PullRequestNewCommitEventArguments();
public record PullRequestReviewAddedEventArguments();
public record PullRequestCommandSendEventArguments();

public interface IGithubActivityHandler
{
    void HandlePullRequestCreation(PullRequestCreateEventArguments arguments);
    void HandlePullRequestNewCommit(PullRequestNewCommitEventArguments arguments);
    void HandlePullRequestReviewAdded(PullRequestReviewAddedEventArguments arguments);
    void HandlePullRequestCommandSend(PullRequestCommandSendEventArguments arguments);
}