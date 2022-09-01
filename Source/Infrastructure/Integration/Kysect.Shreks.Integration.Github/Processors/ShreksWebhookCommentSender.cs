using Kysect.Shreks.Application.Commands.Result;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Integration.Github.Entities;
using Microsoft.Extensions.Logging;
using Octokit.Webhooks;
using Octokit.Webhooks.Events;

namespace Kysect.Shreks.Integration.Github.Processors;

public class ShreksWebhookCommentSender
{
    private readonly IActionNotifier _actionNotifier;
    private readonly ILogger<ShreksWebhookEventProcessorProxy> _logger;

    public ShreksWebhookCommentSender(IActionNotifier actionNotifier, ILogger<ShreksWebhookEventProcessorProxy> logger)
    {
        _actionNotifier = actionNotifier;
        _logger = logger;
    }

    public async Task NotifySubmissionCreated(PullRequestEvent pullRequestEvent, SubmissionDto submission)
    {
        string message = $"Submission {submission.Code} ({submission.SubmissionDate}) was created.";

        await _actionNotifier.SendComment(
            pullRequestEvent,
            pullRequestEvent.PullRequest.Number,
            message);
    }

    public async Task NotifySubmissionUpdate(PullRequestEvent pullRequestEvent, SubmissionDto submission)
    {
        string message = $"Submission {submission.Code} was updated." +
                         $"\nState: {submission.State}" +
                         $"\nDate: {submission.SubmissionDate}";

        await _actionNotifier.SendCommitComment(
            pullRequestEvent,
            pullRequestEvent.PullRequest.Head.Sha,
            message);
    }

    public async Task NotifyAboutCommandProcessingResult(IssueCommentEvent issueCommentEvent, BaseShreksCommandResult result)
    {
        if (!string.IsNullOrEmpty(result.Message))
        {
            await _actionNotifier.SendComment(
                issueCommentEvent,
                issueCommentEvent.Issue.Number,
                result.Message);
        }

        await _actionNotifier.ReactInComments(
            issueCommentEvent,
            issueCommentEvent.Comment.Id,
            result.IsSuccess);
    }

    public async Task NotifyAboutReviewCommandProcessingResult(PullRequestReviewEvent prCommentEvent, BaseShreksCommandResult result)
    {
        if (!string.IsNullOrEmpty(result.Message))
        {
            await _actionNotifier.SendComment(
                prCommentEvent,
                prCommentEvent.PullRequest.Number,
                result.Message);
        }

        await _actionNotifier.ReactInComments(
            prCommentEvent,
            prCommentEvent.Review.Id,
            result.IsSuccess);
    }

    public async Task NotifyPullRequestReviewProcessed(
        PullRequestReviewEvent pullRequestReviewEvent,
        string message = $"Pull request review action handled.")
    {
        await _actionNotifier.SendComment(
            pullRequestReviewEvent,
            pullRequestReviewEvent.PullRequest.Number,
            message);
    }

    public async Task SendExceptionMessageSafe(WebhookEvent webhookEvent, int issueNumber, Exception exception)
    {
        try
        {
            if (exception is ShreksDomainException domainException)
                await _actionNotifier.SendComment(webhookEvent, issueNumber, domainException.Message);
            else
            {
                const string newMessage = $"An error internal error occurred while processing command. Contact support for details.";
                await _actionNotifier.SendComment(webhookEvent, issueNumber, newMessage);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to send exception message to user pull request.");
        }
    }

    public async Task WarnPullRequestMergedWithoutPoints(PullRequestEvent pullRequestEvent, SubmissionDto submissionDto)
    {
        await _actionNotifier.SendComment(
            pullRequestEvent,
            pullRequestEvent.PullRequest.Number,
            $"Warning: pull request was merged, but submission {submissionDto.Code} is not yet rated."
        );
    }
}