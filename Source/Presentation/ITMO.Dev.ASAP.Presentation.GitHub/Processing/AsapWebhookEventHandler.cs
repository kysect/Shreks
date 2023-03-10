using ITMO.Dev.ASAP.Application.Abstractions.Submissions.Models;
using ITMO.Dev.ASAP.Application.Contracts.Github.Queries;
using ITMO.Dev.ASAP.Application.Contracts.GithubEvents;
using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Application.Dto.Submissions;
using ITMO.Dev.ASAP.Application.Dto.Users;
using ITMO.Dev.ASAP.Application.GithubWorkflow.Abstractions.Extensions;
using ITMO.Dev.ASAP.Application.GithubWorkflow.Abstractions.Models;
using ITMO.Dev.ASAP.Application.GithubWorkflow.Abstractions.Notifications;
using ITMO.Dev.ASAP.Commands.Execution;
using ITMO.Dev.ASAP.Commands.Parsers;
using ITMO.Dev.ASAP.Commands.Result;
using ITMO.Dev.ASAP.Commands.SubmissionCommands;
using ITMO.Dev.ASAP.Commands.Tools;
using ITMO.Dev.ASAP.Common.Resources;
using ITMO.Dev.ASAP.Presentation.GitHub.ContextFactories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ITMO.Dev.ASAP.Presentation.GitHub.Processing;

internal class AsapWebhookEventHandler : IAsapWebhookEventHandler
{
    private readonly ISubmissionCommandParser _commandParser;
    private readonly IDefaultSubmissionProvider _defaultSubmissionProvider;
    private readonly IMediator _mediator;

    public AsapWebhookEventHandler(
        ISubmissionCommandParser commandParser,
        IMediator mediator,
        IDefaultSubmissionProvider defaultSubmissionProvider)
    {
        _commandParser = commandParser;
        _mediator = mediator;
        _defaultSubmissionProvider = defaultSubmissionProvider;
    }

    public async Task ProcessPullRequestReopen(
        GithubPullRequestDescriptor prDescriptor,
        ILogger logger,
        IPullRequestCommitEventNotifier eventNotifier,
        CancellationToken cancellationToken)
    {
        UserDto issuer = await GetUserAsync(prDescriptor.Sender, cancellationToken);
        SubmissionDto submission = await GetSubmissionAsync(prDescriptor, cancellationToken);

        var command = new PullRequestReopened.Command(issuer.Id, submission.Id);
        PullRequestReopened.Response response = await _mediator.Send(command, cancellationToken);

        await NotifySubmissionActionMessage(response.Message, eventNotifier, logger);
    }

    public async Task ProcessPullRequestUpdate(
        GithubPullRequestDescriptor prDescriptor,
        ILogger logger,
        IPullRequestCommitEventNotifier eventNotifier,
        CancellationToken cancellationToken)
    {
        UserDto issuer = await GetUserAsync(prDescriptor.Sender, cancellationToken);
        UserDto user = await GetUserAsync(prDescriptor.Repository, cancellationToken);

        AssignmentDto assignment = await GetAssignmentAsync(
            prDescriptor.Organization, prDescriptor.BranchName, cancellationToken);

        var command = new PullRequestUpdated.Command(
            issuer.Id,
            user.Id,
            assignment.Id,
            prDescriptor.Organization,
            prDescriptor.Repository,
            prDescriptor.PrNumber,
            prDescriptor.Payload);

        PullRequestUpdated.Response response = await _mediator.Send(command, cancellationToken);
        SubmissionUpdateResult result = response.Message;

        if (result.IsCreated)
        {
            string message = UserCommandProcessingMessage.SubmissionCreated(result.Submission.ToDisplayString());
            await eventNotifier.SendCommentToPullRequest(message);
        }
        else
        {
            await eventNotifier.NotifySubmissionUpdate(result.Submission, logger);
        }
    }

    public async Task ProcessPullRequestClosed(
        bool merged,
        GithubPullRequestDescriptor prDescriptor,
        ILogger logger,
        IPullRequestCommitEventNotifier eventNotifier,
        CancellationToken cancellationToken)
    {
        UserDto issuer = await GetUserAsync(prDescriptor.Sender, cancellationToken);
        SubmissionDto submission = await GetSubmissionAsync(prDescriptor, cancellationToken);

        var command = new PullRequestClosed.Command(issuer.Id, submission.Id, merged);
        PullRequestClosed.Response response = await _mediator.Send(command, cancellationToken);

        await NotifySubmissionActionMessage(response.Message, eventNotifier, logger);
    }

    public async Task ProcessPullRequestReviewComment(
        string? comment,
        GithubPullRequestDescriptor prDescriptor,
        ILogger logger,
        IPullRequestEventNotifier eventNotifier,
        CancellationToken cancellationToken)
    {
        if (comment is null)
        {
            logger.LogInformation("Review body is null, skipping review comment");
            return;
        }

        if (comment.FirstOrDefault() is not '/')
            return;

        ISubmissionCommand command = _commandParser.Parse(comment);
        await ExecuteCommand(command, prDescriptor, logger, eventNotifier, cancellationToken);

        if (command is not RateCommand)
        {
            string message = UserCommandProcessingMessage.ReviewWithoutRate();
            await eventNotifier.SendCommentToPullRequest(message);
            logger.LogInformation("Notify: {Message}", message);
        }
    }

    public async Task ProcessPullRequestReviewRequestChanges(
        string? reviewBody,
        GithubPullRequestDescriptor prDescriptor,
        ILogger logger,
        IPullRequestEventNotifier eventNotifier,
        CancellationToken cancellationToken)
    {
        UserDto issuer = await GetUserAsync(prDescriptor.Sender, cancellationToken);
        SubmissionDto submission = await GetSubmissionAsync(prDescriptor, cancellationToken);

        var command = new PullRequestChangesRequested.Command(issuer.Id, submission.Id);
        PullRequestChangesRequested.Response response = await _mediator.Send(command, cancellationToken);

        await NotifySubmissionActionMessage(response.Message, eventNotifier, logger);

        if (reviewBody?.FirstOrDefault() is not '/')
            return;

        ISubmissionCommand submissionCommand = _commandParser.Parse(reviewBody);
        await ExecuteCommand(submissionCommand, prDescriptor, logger, eventNotifier, cancellationToken);
    }

    public async Task ProcessPullRequestReviewApprove(
        string? commentBody,
        GithubPullRequestDescriptor prDescriptor,
        ILogger logger,
        IPullRequestEventNotifier eventNotifier,
        CancellationToken cancellationToken)
    {
        UserDto issuer = await GetUserAsync(prDescriptor.Sender, cancellationToken);
        SubmissionDto submission = await GetSubmissionAsync(prDescriptor, cancellationToken);

        var command = new PullRequestApproved.Command(issuer.Id, submission.Id);
        PullRequestApproved.Response response = await _mediator.Send(command, cancellationToken);

        await NotifySubmissionActionMessage(response.Message, eventNotifier, logger);

        if (commentBody?.FirstOrDefault() is not '/')
            return;

        ISubmissionCommand submissionCommand = _commandParser.Parse(commentBody);
        await ExecuteCommand(submissionCommand, prDescriptor, logger, eventNotifier, cancellationToken);
    }

    public async Task ProcessIssueCommentCreate(
        string issueCommentBody,
        GithubPullRequestDescriptor prDescriptor,
        ILogger logger,
        IPullRequestCommentEventNotifier eventNotifier,
        CancellationToken cancellationToken)
    {
        if (issueCommentBody.FirstOrDefault() is not '/')
            return;

        ISubmissionCommand command = _commandParser.Parse(issueCommentBody);
        ICommandResult result = await ExecuteCommand(
            command, prDescriptor, logger, eventNotifier, cancellationToken);

        await eventNotifier.ReactToUserComment(result.IsSuccess);
    }

    private static async Task NotifySubmissionActionMessage(
        SubmissionActionMessageDto message,
        IPullRequestEventNotifier eventNotifier,
        ILogger logger)
    {
        await eventNotifier.SendCommentToPullRequest(message.Message);
        logger.LogInformation("Notify: {Message}", message.Message);
    }

    private async Task<UserDto> GetUserAsync(
        string username,
        CancellationToken cancellationToken)
    {
        var query = new GetGithubUser.Query(username);
        GetGithubUser.Response response = await _mediator.Send(query, cancellationToken);

        return response.User;
    }

    private async Task<AssignmentDto> GetAssignmentAsync(
        string organizationName,
        string branchName,
        CancellationToken cancellationToken)
    {
        var query = new GetGitHubAssignment.Query(organizationName, branchName);
        GetGitHubAssignment.Response response = await _mediator.Send(query, cancellationToken);

        return response.Assignment;
    }

    private async Task<SubmissionDto> GetLastPullRequestSubmissionAsync(
        Guid userId,
        Guid assignmentId,
        long pullRequestNumber,
        CancellationToken cancellationToken)
    {
        var query = new GetLastPullRequestSubmission.Query(userId, assignmentId, pullRequestNumber);
        GetLastPullRequestSubmission.Response response = await _mediator.Send(query, cancellationToken);

        return response.Submission;
    }

    private async Task<SubmissionDto> GetSubmissionAsync(
        GithubPullRequestDescriptor descriptor,
        CancellationToken cancellationToken)
    {
        UserDto user = await GetUserAsync(descriptor.Repository, cancellationToken);

        AssignmentDto assignment = await GetAssignmentAsync(
            descriptor.Organization, descriptor.BranchName, cancellationToken);

        return await GetLastPullRequestSubmissionAsync(
            user.Id, assignment.Id, descriptor.PrNumber, cancellationToken);
    }

    private async Task<ICommandResult> ExecuteCommand(
        ISubmissionCommand command,
        GithubPullRequestDescriptor prDescriptor,
        ILogger logger,
        IPullRequestEventNotifier eventNotifier,
        CancellationToken cancellationToken)
    {
        var contextFactory = new SubmissionCommandContextFactory(prDescriptor, _mediator, _defaultSubmissionProvider);
        var executor = new SubmissionCommandExecutor(contextFactory, logger);

        ICommandResult result = await executor.ExecuteAsync(command, cancellationToken);

        await eventNotifier.SendCommentToPullRequest(result.Message);
        logger.LogInformation("Notify: {Message}", result.Message);

        return result;
    }
}