using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.GithubWorkflow.Abstractions;
using Kysect.Shreks.Core.Submissions;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Application.GithubWorkflow.Extensions;

public static class PullRequestCommitEventNotifierExtensions
{
    public static async Task NotifySubmissionUpdate(
        this IPullRequestEventNotifier pullRequestCommitEventNotifier,
        Submission submission,
        ILogger logger,
        bool sendComment = false)
    {
        string message = $"Submission {submission.Code} was updated." +
                         $"\nState: {submission.State}" +
                         $"\nDate: {submission.SubmissionDate}";

        if (sendComment)
        {
            logger.LogInformation("Notify comment posted into PR: " + message);
            await pullRequestCommitEventNotifier.SendCommentToPullRequest(message);
        }
    }

    public static async Task NotifySubmissionUpdate(
        this IPullRequestCommitEventNotifier pullRequestCommitEventNotifier,
        Submission submission,
        ILogger logger,
        bool sendComment = false,
        bool sendCommitComment = false)
    {
        string message = $"Submission {submission.Code} was updated." +
                         $"\nState: {submission.State}" +
                         $"\nDate: {submission.SubmissionDate}";

        if (sendComment)
        {
            logger.LogInformation("Notify comment posted into PR: " + message);
            await pullRequestCommitEventNotifier.SendCommentToPullRequest(message);
        }

        if (sendCommitComment)
        {
            logger.LogInformation("Notify posted as commit comment: " + message);
            await pullRequestCommitEventNotifier.SendCommentToTriggeredCommit(message);
        }
    }

    public static async Task NotifySubmissionUpdate(
        this IPullRequestCommitEventNotifier pullRequestCommitEventNotifier,
        SubmissionDto submission,
        ILogger logger,
        bool sendComment = false,
        bool sendCommitComment = false)
    {
        string message = $"Submission {submission.Code} was updated." +
                         $"\nState: {submission.State}" +
                         $"\nDate: {submission.SubmissionDate}";

        if (sendComment)
        {
            logger.LogInformation("Notify comment posted into PR: " + message);
            await pullRequestCommitEventNotifier.SendCommentToPullRequest(message);
        }

        if (sendCommitComment)
        {
            logger.LogInformation("Notify posted as commit comment: " + message);
            await pullRequestCommitEventNotifier.SendCommentToTriggeredCommit(message);
        }
    }
}