﻿using Kysect.Shreks.Application.Commands.Commands;
using Kysect.Shreks.Application.Commands.Processors;
using Kysect.Shreks.Application.Commands.Result;
using Kysect.Shreks.Application.GithubWorkflow.Abstractions;
using Kysect.Shreks.Application.GithubWorkflow.Abstractions.Models;
using Kysect.Shreks.Application.GithubWorkflow.Extensions;
using Kysect.Shreks.Application.GithubWorkflow.Submissions;
using Kysect.Shreks.Common.Resources;
using Kysect.Shreks.Core.Models;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Core.Users;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Application.GithubWorkflow.SubmissionStateMachines;

public class ReviewOnlyGithubSubmissionStateMachine : IGithubSubmissionStateMachine
{
    private readonly GithubSubmissionService _githubSubmissionService;
    private readonly SubmissionService _shreksCommandProcessor;
    private readonly ShreksCommandProcessor _commandProcessor;
    private readonly ILogger _logger;
    private readonly IPullRequestEventNotifier _eventNotifier;

    public ReviewOnlyGithubSubmissionStateMachine(
        SubmissionService shreksCommandProcessor,
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

    public async Task ProcessPullRequestReviewApprove(IShreksCommand? command, GithubPullRequestDescriptor prDescriptor, User sender)
    {
        Submission submission = await _githubSubmissionService.GetLastSubmissionByPr(prDescriptor);

        double? points = submission.Points?.Value;

        // TODO: check case when command is not rate
        if (command is not null)
        {
            BaseShreksCommandResult result = await _commandProcessor.ProcessBaseCommandSafe(command, CancellationToken.None);
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
                BaseShreksCommandResult result = await _commandProcessor.ProcessBaseCommandSafe(command, CancellationToken.None);
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
            BaseShreksCommandResult result = await _commandProcessor.ProcessBaseCommandSafe(command, CancellationToken.None);
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

    public async Task ProcessPullRequestReopen(bool? isMerged, GithubPullRequestDescriptor prDescriptor, User sender)
    {
        if (isMerged.HasValue && isMerged == false)
            await ChangeSubmissionState(SubmissionState.Active, prDescriptor, sender);
    }

    public async Task ProcessPullRequestClosed(bool isMerged, GithubPullRequestDescriptor prDescriptor, User sender)
    {
        var state = isMerged ? SubmissionState.Completed : SubmissionState.Inactive;
        var submission = await ChangeSubmissionState(state, prDescriptor, sender);

        if (isMerged && submission.Points is null)
            await _eventNotifier.SendCommentToPullRequest(UserCommandProcessingMessage.MergePullRequestWithoutRate(submission.Code));
    }

    private async Task<Submission> ChangeSubmissionState(SubmissionState state, GithubPullRequestDescriptor githubPullRequestDescriptor, User sender)
    {
        Submission submission = await _githubSubmissionService.GetLastSubmissionByPr(githubPullRequestDescriptor);
        Submission completedSubmission = await _shreksCommandProcessor.UpdateSubmissionState(submission.Id, sender.Id, state, CancellationToken.None);

        await _eventNotifier.NotifySubmissionUpdate(completedSubmission, _logger);
        return completedSubmission;
    }
}