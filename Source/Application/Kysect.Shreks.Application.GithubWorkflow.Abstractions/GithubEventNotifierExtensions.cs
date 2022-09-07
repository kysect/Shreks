using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Common.Exceptions;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Application.GithubWorkflow.Abstractions;

public static class GithubEventNotifierExtensions
{
    public static async Task SendExceptionMessageSafe(this IPullRequestEventNotifier pullRequestEventNotifier, Exception exception, ILogger logger)
    {
        try
        {
            if (exception is ShreksDomainException domainException)
            {
                await pullRequestEventNotifier.SendCommentToPullRequest(domainException.Message);
            }
            else
            {
                const string newMessage = "An internal error occurred while processing command. Contact support for details.";
                await pullRequestEventNotifier.SendCommentToPullRequest(newMessage);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to send exception message to user pull request.");
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