using ITMO.Dev.ASAP.Application.GithubWorkflow.Abstractions.Notifications;
using ITMO.Dev.ASAP.Common.Exceptions;
using Microsoft.Extensions.Logging;

namespace ITMO.Dev.ASAP.Application.GithubWorkflow.Abstractions.Extensions;

public static class GithubEventNotifierExtensions
{
    public static async Task SendExceptionMessageSafe(
        this IPullRequestEventNotifier pullRequestEventNotifier,
        Exception exception,
        ILogger logger)
    {
        try
        {
            if (exception is DomainException domainException)
            {
                await pullRequestEventNotifier.SendCommentToPullRequest(domainException.Message);
            }
            else
            {
                const string newMessage =
                    "An internal error occurred while processing command. Contact support for details.";
                await pullRequestEventNotifier.SendCommentToPullRequest(newMessage);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to send exception message to user pull request.");
        }
    }
}