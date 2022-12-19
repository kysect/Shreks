using Kysect.Shreks.Application.GithubWorkflow.Abstractions;
using Kysect.Shreks.Core.Submissions;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Application.GithubWorkflow.Extensions;

public static class PullRequestCommitEventNotifierExtensions
{
    public static async Task NotifySubmissionUpdate(
        this IPullRequestEventNotifier pullRequestCommitEventNotifier,
        Submission submission,
        ILogger logger)
    {
        string message = $"Submission {submission.Code} was updated." +
                         $"\nState: {submission.State.Kind}" +
                         $"\nDate: {submission.SubmissionDate}";

        logger.LogInformation("Notify comment posted into PR: " + message);
        await pullRequestCommitEventNotifier.SendCommentToPullRequest(message);
    }

    public static async Task NotifySubmissionUpdate(
        this IPullRequestCommitEventNotifier pullRequestCommitEventNotifier,
        Submission submission,
        ILogger logger)
    {
        string message = $"Submission {submission.Code} was updated." +
                         $"\nState: {submission.State.Kind}" +
                         $"\nDate: {submission.SubmissionDate}";

        logger.LogInformation("Notify posted as commit comment: " + message);
        await pullRequestCommitEventNotifier.SendCommentToTriggeredCommit(message);
    }
}