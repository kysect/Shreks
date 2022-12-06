using Kysect.Shreks.Application.Commands.Commands;
using Kysect.Shreks.Application.Commands.Processors;
using Kysect.Shreks.Application.Commands.Result;
using Kysect.Shreks.Application.GithubWorkflow.Abstractions;
using Kysect.Shreks.Application.GithubWorkflow.Abstractions.Models;
using Kysect.Shreks.Application.GithubWorkflow.Extensions;
using Kysect.Shreks.Application.GithubWorkflow.Submissions;
using Kysect.Shreks.Common.Resources;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Core.Users;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Application.GithubWorkflow.SubmissionStateMachines;

public class ReviewOnlyGithubSubmissionStateMachine : IGithubSubmissionStateMachine
{
    private readonly GithubSubmissionService _githubSubmissionService;
    private readonly ISubmissionService _shreksCommandProcessor;
    private readonly ShreksCommandProcessor _commandProcessor;
    private readonly ILogger _logger;
    private readonly IPullRequestEventNotifier _eventNotifier;

    public ReviewOnlyGithubSubmissionStateMachine(
        ISubmissionService shreksCommandProcessor,
        ShreksCommandProcessor commandProcessor,
        GithubSubmissionService githubSubmissionService,
        ILogger logger,
        IPullRequestEventNotifier eventNotifier)
    {
        _githubSubmissionService = githubSubmissionService;
        _shreksCommandProcessor = shreksCommandProcessor;
        _commandProcessor = commandProcessor;
        _logger = logger;
        _eventNotifier = eventNotifier;
    }

    public async Task ProcessPullRequestReviewApprove(
        IShreksCommand? command,
        GithubPullRequestDescriptor prDescriptor,
        User sender)
    {
        Submission submission = await _githubSubmissionService.GetLastSubmissionByPr(prDescriptor);

        double? points = submission.Points?.Value;

        // TODO: check case when command is not rate
        if (command is not null)
        {
            BaseShreksCommandResult result = await _commandProcessor.ProcessBaseCommandSafe(command, default);
            await _eventNotifier.SendCommentToPullRequest(result.Message);
            _logger.LogInformation("Notify: " + result.Message);
            return;
        }

        string message;

        switch (points)
        {
            case null:
            {
                command = new RateCommand(ratingPercent: 100, extraPoints: 0);
                BaseShreksCommandResult result = await _commandProcessor.ProcessBaseCommandSafe(command, default);
                message = result.Message;
                break;
            }
            default:
            {
                message = UserCommandProcessingMessage.ReviewRatedSubmission(points.Value);
                break;
            }
        }

        await _eventNotifier.SendCommentToPullRequest(message);
        _logger.LogInformation("Notify: " + message);
    }

    public async Task ProcessPullRequestReviewRequestChanges(IShreksCommand? command)
    {
        if (command is null)
            command = new RateCommand(ratingPercent: 0, extraPoints: 0);

        BaseShreksCommandResult result = await _commandProcessor.ProcessBaseCommandSafe(command, CancellationToken.None);
        await _eventNotifier.SendCommentToPullRequest(result.Message);
        _logger.LogInformation("Notify: " + result.Message);
    }

    public async Task ProcessPullRequestReviewComment(IShreksCommand? command)
    {
        if (command is not null)
        {
            BaseShreksCommandResult result = await _commandProcessor.ProcessBaseCommandSafe(command, default);
            await _eventNotifier.SendCommentToPullRequest(result.Message);
            _logger.LogInformation("Notify: " + result.Message);
        }

        if (command is not RateCommand)
        {
            string message = UserCommandProcessingMessage.ReviewWithoutRate();
            await _eventNotifier.SendCommentToPullRequest(message);
            _logger.LogInformation("Notify: " + message);
        }
    }

    public async Task ProcessPullRequestReopen(GithubPullRequestDescriptor prDescriptor, User sender)
    {
        Submission submission = await GetSubmission(prDescriptor);
        submission = await _shreksCommandProcessor.ActivateSubmissionAsync(submission.Id, sender.Id, default);
        await _eventNotifier.NotifySubmissionUpdate(submission, _logger);
    }

    public async Task ProcessPullRequestClosed(bool isMerged, GithubPullRequestDescriptor prDescriptor, User sender)
    {
        Submission submission = await GetSubmission(prDescriptor);

        submission = isMerged
            ? await _shreksCommandProcessor.CompleteSubmissionAsync(submission.Id, sender.Id, default)
            : await _shreksCommandProcessor.DeactivateSubmissionAsync(submission.Id, sender.Id, default);

        if (!isMerged || submission.Points is not null)
            return;

        string message = UserCommandProcessingMessage.MergePullRequestWithoutRate(submission.Code);
        await _eventNotifier.SendCommentToPullRequest(message);
        await _eventNotifier.NotifySubmissionUpdate(submission, _logger);
    }

    private Task<Submission> GetSubmission(GithubPullRequestDescriptor githubPullRequestDescriptor)
    {
        return _githubSubmissionService.GetLastSubmissionByPr(githubPullRequestDescriptor);
    }
}