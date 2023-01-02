using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.GithubWorkflow.Abstractions.Notifications;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Application.GithubWorkflow.Abstractions.Extensions;

public static class PullRequestCommitEventNotifierExtensions
{
    public static async Task NotifySubmissionUpdate(
        this IPullRequestEventNotifier pullRequestCommitEventNotifier,
        SubmissionRateDto submission,
        ILogger logger)
    {
        string message = $"Submission {submission.Code} was updated." +
                         $"\nState: {submission.State}" +
                         $"\nDate: {submission.SubmissionDate}";

        logger.LogInformation("Notify comment posted into PR: " + message);
        await pullRequestCommitEventNotifier.SendCommentToPullRequest(message);
    }

    public static async Task NotifySubmissionUpdate(
        this IPullRequestCommitEventNotifier pullRequestCommitEventNotifier,
        SubmissionRateDto submission,
        ILogger logger)
    {
        string message = $"Submission {submission.Code} was updated." +
                         $"\nState: {submission.State}" +
                         $"\nDate: {submission.SubmissionDate}";

        logger.LogInformation("Notify posted as commit comment: " + message);
        await pullRequestCommitEventNotifier.SendCommentToTriggeredCommit(message);
    }
}