using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Application.Commands.Commands;
using Kysect.Shreks.Application.Commands.Parsers;
using Kysect.Shreks.Application.Commands.Processors;
using Kysect.Shreks.Application.Commands.Result;
using Kysect.Shreks.Application.Dto.Github;
using Kysect.Shreks.Application.Extensions;
using Kysect.Shreks.Application.GithubWorkflow.Abstractions;
using Kysect.Shreks.Application.GithubWorkflow.Extensions;
using Kysect.Shreks.DataAccess.Abstractions;
using Microsoft.Extensions.Logging;
using Kysect.Shreks.Application.GithubWorkflow.Models;
using Kysect.Shreks.Application.GithubWorkflow.Submissions;
using Kysect.Shreks.Application.GithubWorkflow.SubmissionStateMachines;

namespace Kysect.Shreks.Application.GithubWorkflow;

public class ShreksWebhookEventProcessor : IShreksWebhookEventProcessor
{
    private readonly IShreksCommandParser _commandParser;
    private readonly IShreksDatabaseContext _context;
    private readonly ITableUpdateQueue _queue;
    private readonly GithubSubmissionFactory _githubSubmissionFactory;
    private readonly SubmissionService _shreksCommandProcessor;
    private readonly IGithubSubmissionStateMachine _submissionStateMachine;

    public ShreksWebhookEventProcessor(
        IShreksCommandParser commandParser,
        IShreksDatabaseContext context,
        ITableUpdateQueue queue,
        GithubSubmissionFactory githubSubmissionFactory)
    {
        _commandParser = commandParser;
        _context = context;
        _queue = queue;
        _githubSubmissionFactory = githubSubmissionFactory;

        _shreksCommandProcessor = new SubmissionService(_context, _queue);
        _submissionStateMachine = new OneStageGithubSubmissionStateMachine(_context, _shreksCommandProcessor);
    }

    public async Task ProcessPullRequestReopen(bool? isMerged, GithubPullRequestDescriptor prDescriptor, ILogger logger, IPullRequestCommitEventNotifier eventNotifier)
    {
        await _submissionStateMachine.ProcessPullRequestReopen(isMerged, prDescriptor, logger, eventNotifier);
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
        await _submissionStateMachine.ProcessPullRequestClosed(merged, prDescriptor, logger, eventNotifier);
    }

    public async Task ProcessPullRequestReviewComment(string? comment, GithubPullRequestDescriptor prDescriptor, ILogger logger, IPullRequestEventNotifier eventNotifier)
    {
        if (comment is null)
        {
            logger.LogInformation("Review body is null, skipping review comment");
            return;
        }

        IShreksCommand? command = null;

        if (comment.FirstOrDefault() == '/')
            command = _commandParser.Parse(comment);

        await _submissionStateMachine.ProcessPullRequestReviewComment(command, CreateCommandProcessor(prDescriptor, logger), logger, eventNotifier);
    }

    public async Task ProcessPullRequestReviewRequestChanges(string? reviewBody, GithubPullRequestDescriptor prDescriptor, ILogger logger, IPullRequestEventNotifier eventNotifier)
    {
        string changesComment = reviewBody ?? string.Empty;
        IShreksCommand? requestChangesCommand = null;

        if (changesComment.FirstOrDefault() == '/')
            requestChangesCommand = _commandParser.Parse(changesComment);

        await _submissionStateMachine.ProcessPullRequestReviewRequestChanges(requestChangesCommand, CreateCommandProcessor(prDescriptor, logger), logger, eventNotifier);
    }

    public async Task ProcessPullRequestReviewApprove(string? commentBody, GithubPullRequestDescriptor prDescriptor, ILogger logger, IPullRequestEventNotifier eventNotifier)
    {
        string approveComment = commentBody ?? string.Empty;
        IShreksCommand? command = null;

        if (approveComment.FirstOrDefault() == '/')
            command = _commandParser.Parse(approveComment);

        await _submissionStateMachine.ProcessPullRequestReviewApprove(command, CreateCommandProcessor(prDescriptor, logger), prDescriptor, logger, eventNotifier);
    }

    public async Task ProcessIssueCommentCreate(
        string issueCommentBody,
        GithubPullRequestDescriptor prDescriptor,
        ILogger logger,
        IPullRequestCommentEventNotifier eventNotifier)
    {
        if (issueCommentBody.FirstOrDefault() != '/')
            return;

        IShreksCommand command = _commandParser.Parse(issueCommentBody);
        ShreksCommandProcessor commandProcessor = CreateCommandProcessor(prDescriptor, logger);
        BaseShreksCommandResult result = await commandProcessor.ProcessBaseCommandSafe(command, CancellationToken.None);
        if (!string.IsNullOrEmpty(result.Message))
            await eventNotifier.SendCommentToPullRequest(result.Message);

        await eventNotifier.ReactToUserComment(result.IsSuccess);
    }

    private ShreksCommandProcessor CreateCommandProcessor(GithubPullRequestDescriptor pullRequestDescriptor, ILogger repositoryLogger)
    {
        var commentContextFactory = new PullRequestCommentContextFactory(pullRequestDescriptor, _githubSubmissionFactory, _context, _shreksCommandProcessor);
        return new ShreksCommandProcessor(commentContextFactory, repositoryLogger);
    }
}