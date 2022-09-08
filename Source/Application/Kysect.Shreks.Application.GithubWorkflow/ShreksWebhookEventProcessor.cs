using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Application.CommandProcessing;
using Kysect.Shreks.Application.Commands.Commands;
using Kysect.Shreks.Application.Commands.Parsers;
using Kysect.Shreks.Application.Commands.Result;
using Kysect.Shreks.Application.Dto.Github;
using Kysect.Shreks.Application.GithubWorkflow.Abstractions;
using Kysect.Shreks.Application.GithubWorkflow.Extensions;
using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.Extensions;
using Kysect.Shreks.Core.Specifications.Submissions;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;
using Kysect.Shreks.Core.Submissions;
using Microsoft.EntityFrameworkCore;
using Kysect.Shreks.Core.Models;

namespace Kysect.Shreks.Application.GithubWorkflow;

public class ShreksWebhookEventProcessor : IShreksWebhookEventProcessor
{
    private readonly IShreksCommandParser _commandParser;
    private readonly IMediator _mediator;
    private readonly IShreksDatabaseContext _context;
    private readonly ITableUpdateQueue _queue;
    private readonly GithubSubmissionFactory _githubSubmissionFactory;

    public ShreksWebhookEventProcessor(
        IShreksCommandParser commandParser,
        IMediator mediator,
        IShreksDatabaseContext context,
        ITableUpdateQueue queue,
        GithubSubmissionFactory githubSubmissionFactory)
    {
        _commandParser = commandParser;
        _mediator = mediator;
        _context = context;
        _queue = queue;
        _githubSubmissionFactory = githubSubmissionFactory;
    }

    public async Task ProcessPullRequestReopen(bool? isMerged, GithubPullRequestDescriptor prDescriptor, ILogger logger, IPullRequestCommitEventNotifier eventNotifier)
    {
        if (isMerged.HasValue && isMerged == false)
            await ChangeSubmissionState(SubmissionState.Active, prDescriptor, logger, eventNotifier);
    }

    public async Task ProcessPullRequestUpdate(GithubPullRequestDescriptor prDescriptor, ILogger logger, IPullRequestCommitEventNotifier eventNotifier, CancellationToken cancellationToken)
    {
        GithubSubmissionCreationResult result = await _githubSubmissionFactory.CreateOrUpdateGithubSubmission(prDescriptor, cancellationToken);
        _queue.EnqueueSubmissionsQueueUpdate(result.Submission.GetCourseId(), result.Submission.GetGroupId());

        if (result.IsCreated)
        {
            string message = $"Submission {result.Submission.Code} ({result.Submission.SubmissionDate}) was created.";
            await eventNotifier.SendCommentToPullRequest(message);
        }
        else
        {
            await eventNotifier.NotifySubmissionUpdate(result.Submission, logger, false, true);
        }
    }

    public async Task ProcessPullRequestClosed(bool merged, GithubPullRequestDescriptor prDescriptor, ILogger logger, IPullRequestCommitEventNotifier eventNotifier)
    {
        var state = merged ? SubmissionState.Completed : SubmissionState.Inactive;
        var submission = await ChangeSubmissionState(state, prDescriptor, logger, eventNotifier);

        if (merged && submission.Points is null)
        {
            string message = $"Warning: pull request was merged, but submission {submission.Code} is not yet rated.";
            await eventNotifier.SendCommentToPullRequest(message);
        }
    }

    private async Task<Submission> ChangeSubmissionState(
        SubmissionState state,
        GithubPullRequestDescriptor githubPullRequestDescriptor,
        ILogger repositoryLogger,
        IPullRequestCommitEventNotifier pullRequestCommitEventNotifier)
    {
        User? user = await _context.UserAssociations.FindUserByGithubUsername(githubPullRequestDescriptor.Sender)
                      ?? throw new EntityNotFoundException($"Entity of type User with login {githubPullRequestDescriptor.Sender}");
        GithubSubmission submission = await GetGithubSubmissionAsync(
            githubPullRequestDescriptor.Organization,
            githubPullRequestDescriptor.Repository,
            githubPullRequestDescriptor.PrNumber);

        var shreksCommandProcessor = new ShreksCommandProcessor(_context, _queue);
        Submission completedSubmission = await shreksCommandProcessor.UpdateSubmission(submission.Id, user.Id, state, CancellationToken.None);

        var pullRequestCommentEventNotifier = pullRequestCommitEventNotifier;
        await pullRequestCommentEventNotifier.NotifySubmissionUpdate(completedSubmission, repositoryLogger, true, false);
        return completedSubmission;
    }

    public async Task ProcessPullRequestReviewComment(string? comment, GithubPullRequestDescriptor prDescriptor, ILogger logger, IPullRequestEventNotifier eventNotifier)
    {
        IShreksCommand? command = null;

        if (comment is null)
        {
            logger.LogInformation("Review body is null, skipping review comment");
            return;
        }

        if (comment.FirstOrDefault() == '/')
        {
            command = _commandParser.Parse(comment);
            var result = await ProceedCommandAsync(command, prDescriptor, logger);
            await eventNotifier.SendCommentToPullRequest(result.Message);
            logger.LogInformation("Notify: " + result.Message);
        }

        if (command is not RateCommand)
        {
            string message = $"Review proceeded, but submission is not yet rated and still will be presented in queue";
            await eventNotifier.SendCommentToPullRequest(message);
            logger.LogInformation("Notify: " + message);
        }
    }

    public async Task ProcessPullRequestReviewRequestChanges(string? reviewBody, GithubPullRequestDescriptor prDescriptor, ILogger logger, IPullRequestEventNotifier eventNotifier)
    {
        var changesComment = reviewBody ?? string.Empty;
        IShreksCommand requestChangesCommand;

        if (changesComment.FirstOrDefault() == '/')
        {
            requestChangesCommand = _commandParser.Parse(changesComment);
        }
        else
        {
            requestChangesCommand = new RateCommand(0, 0);
        }

        var result = await ProceedCommandAsync(requestChangesCommand, prDescriptor, logger);
        await eventNotifier.SendCommentToPullRequest(result.Message);
        logger.LogInformation("Notify: " + result.Message);
    }

    public async Task ProcessPullRequestReviewApprove(string? commentBody, GithubPullRequestDescriptor prDescriptor, ILogger logger, IPullRequestEventNotifier eventNotifier)
    {
        string approveComment = commentBody ?? string.Empty;
        IShreksCommand approveCommand;

        if (approveComment.FirstOrDefault() == '/')
        {
            approveCommand = _commandParser.Parse(approveComment);
        }
        else
        {
            approveCommand = new RateCommand(100, 0);
        }

        var githubSubmissionService = new GithubSubmissionService(_context);
        Submission submission = await githubSubmissionService.GetLastSubmissionByPr(prDescriptor);
        double? points = submission.Points?.Value;

        switch (points)
        {
            case null:
                BaseShreksCommandResult result = await ProceedCommandAsync(approveCommand, prDescriptor, logger);
                await eventNotifier.SendCommentToPullRequest(result.Message);
                logger.LogInformation("Notify: " + result.Message);
                break;

            case 100:
                {
                    var message = "Review successfully processed, but submission already has 100 points";
                    await eventNotifier.SendCommentToPullRequest(message);
                    logger.LogInformation("Notify: " + message);
                    break;
                }
            default:
                {
                    var message = $"Submission is already rated with {points} points. If you want to change it, please use /update command.";
                    await eventNotifier.SendCommentToPullRequest(message);
                    logger.LogInformation("Notify: " + message);
                    break;
                }
        }
    }

    public async Task ProcessIssueCommentCreate(string issueCommentBody, GithubPullRequestDescriptor prDescriptor,
        ILogger logger,
        IPullRequestCommentEventNotifier eventNotifier)
    {
        if (issueCommentBody.FirstOrDefault() != '/')
            return;

        IShreksCommand command = _commandParser.Parse(issueCommentBody);
        var result = await ProceedCommandAsync(command, prDescriptor, logger);
        if (!string.IsNullOrEmpty(result.Message))
            await eventNotifier.SendCommentToPullRequest(result.Message);

        await eventNotifier.ReactToUserComment(result.IsSuccess);
    }

    private async Task<BaseShreksCommandResult> ProceedCommandAsync(IShreksCommand command, GithubPullRequestDescriptor pullRequestDescriptor, ILogger repositoryLogger)
    {
        var contextCreator = new PullRequestCommentContextFactory(_mediator, pullRequestDescriptor, repositoryLogger, _githubSubmissionFactory, _context);
        var processor = new GithubCommandProcessor(contextCreator, repositoryLogger, CancellationToken.None);
        BaseShreksCommandResult result;

        try
        {
            result = await command.AcceptAsync(processor);
        }
        catch (Exception e)
        {
            const string message = $"An internal error occurred while processing command. Contact support for details.";
            repositoryLogger.LogError(e, message);
            result = new BaseShreksCommandResult(false, message);
        }

        return result;
    }

    private async Task<GithubSubmission> GetGithubSubmissionAsync(string organization, string repository, long prNumber)
    {
        var spec = new FindLatestGithubSubmission(organization, repository, prNumber);

        var submission = await _context.SubmissionAssociations
            .WithSpecification(spec)
            .FirstOrDefaultAsync();

        if (submission is null)
            throw new EntityNotFoundException("No submission found");

        return submission;
    }
}