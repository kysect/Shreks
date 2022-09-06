using Kysect.Shreks.Application.Abstractions.Github.Commands;
using Kysect.Shreks.Application.Abstractions.Github.Queries;
using Kysect.Shreks.Application.Abstractions.Submissions.Commands;
using Kysect.Shreks.Application.Abstractions.Submissions.Queries;
using Kysect.Shreks.Application.Commands.Commands;
using Kysect.Shreks.Application.Commands.Parsers;
using Kysect.Shreks.Application.Commands.Result;
using Kysect.Shreks.Application.Dto.Github;
using Kysect.Shreks.Application.Dto.Study;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Integration.Github.Applicaiton;

public class ShreksWebhookEventProcessor : IShreksWebhookEventProcessor
{
    private readonly IShreksCommandParser _commandParser;
    private readonly IMediator _mediator;

    public ShreksWebhookEventProcessor(IShreksCommandParser commandParser, IMediator mediator)
    {
        _commandParser = commandParser;
        _mediator = mediator;
    }

    public async Task ProcessPullRequestReopenWebhookAsync(GithubPullRequestDescriptor pullRequestDescriptor, ILogger repositoryLogger, IPullRequestCommitEventNotifier pullRequestCommitEventNotifier, bool? isMerged)
    {
        if (isMerged.HasValue && isMerged == false)
            await ChangeSubmissionState(SubmissionStateDto.Active, pullRequestDescriptor, repositoryLogger, pullRequestCommitEventNotifier);
    }

    public async Task ProcessPullRequestUpdateWebhookAsync(GithubPullRequestDescriptor pullRequestDescriptor, ILogger repositoryLogger, IPullRequestCommitEventNotifier pullRequestCommitEventNotifier, CancellationToken cancellationToken)
    {
        var subjectCourseId = await GetSubjectCourseByOrganization(pullRequestDescriptor.Organization, cancellationToken);
        var userId = await GetUserByGithubLogin(pullRequestDescriptor.Sender, cancellationToken);
        var assignmentId = await GetAssignmentByBranchAndSubjectCourse(pullRequestDescriptor.BranchName, subjectCourseId);

        var command = new CreateOrUpdateGithubSubmission.Command(userId, assignmentId, pullRequestDescriptor);

        (bool isCreated, SubmissionDto? submissionDto) = await _mediator.Send(command, cancellationToken);
        if (isCreated)
        {
            string message = $"Submission {submissionDto.Code} ({submissionDto.SubmissionDate}) was created.";
            await pullRequestCommitEventNotifier.SendCommentToPullRequest(message);
        }
        else
        {
            await pullRequestCommitEventNotifier.NotifySubmissionUpdate(submissionDto, repositoryLogger, false, true);
        }
    }

    public async Task ProcessPullRequestClosedWebhookAsync(GithubPullRequestDescriptor pullRequestDescriptor, ILogger repositoryLogger, IPullRequestCommitEventNotifier pullRequestCommitEventNotifier, bool merged)
    {
        var state = merged ? SubmissionStateDto.Completed : SubmissionStateDto.Inactive;
        var submission = await ChangeSubmissionState(state, pullRequestDescriptor, repositoryLogger, pullRequestCommitEventNotifier);

        if (merged && submission.Points is null)
        {
            string message = $"Warning: pull request was merged, but submission {submission.Code} is not yet rated.";
            await pullRequestCommitEventNotifier.SendCommentToPullRequest(message);
        }
    }

    private async Task<SubmissionDto> ChangeSubmissionState(
        SubmissionStateDto state,
        GithubPullRequestDescriptor githubPullRequestDescriptor,
        ILogger repositoryLogger,
        IPullRequestCommitEventNotifier pullRequestCommitEventNotifier)
    {
        var user = await GetUserByGithubLogin(githubPullRequestDescriptor.Sender, CancellationToken.None);
        var submission = await GetGithubSubmissionAsync(
            githubPullRequestDescriptor.Sender,
            githubPullRequestDescriptor.Repository,
            githubPullRequestDescriptor.PrNumber);

        var competeSubmissionCommand = new UpdateSubmissionState.Command(user, submission.Id, state);

        var response = await _mediator.Send(competeSubmissionCommand, CancellationToken.None);
        var pullRequestCommentEventNotifier = pullRequestCommitEventNotifier;
        await pullRequestCommentEventNotifier.NotifySubmissionUpdate(response.Submission, repositoryLogger, true, false);

        return response.Submission;
    }

    public async Task ProcessPullRequestReviewCommentWebhookAsync(GithubPullRequestDescriptor pullRequestDescriptor, ILogger repositoryLogger, IPullRequetsEventNotifier pullRequestEventNotifier, string? comment)
    {
        IShreksCommand? command = null;

        if (comment is null)
        {
            repositoryLogger.LogInformation("Review body is null, skipping review comment");
            return;
        }

        if (comment.FirstOrDefault() == '/')
        {
            command = _commandParser.Parse(comment);
            var result = await ProceedCommandAsync(command, pullRequestDescriptor, repositoryLogger);
            await pullRequestEventNotifier.SendCommentToPullRequest(result.Message);
            repositoryLogger.LogInformation("Notify: " + result.Message);
        }

        if (command is not RateCommand)
        {
            string message = $"Review proceeded, but submission is not yet rated and still will be presented in queue";
            await pullRequestEventNotifier.SendCommentToPullRequest(message);
            repositoryLogger.LogInformation("Notify: " + message);
        }
    }

    public async Task ProcessPullRequestReviewRequestChangesWebhookAsync(GithubPullRequestDescriptor pullRequestDescriptor, ILogger repositoryLogger, IPullRequetsEventNotifier pullRequestEventNotifier, string? reviewBody)
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

        var result = await ProceedCommandAsync(requestChangesCommand, pullRequestDescriptor, repositoryLogger);
        await pullRequestEventNotifier.SendCommentToPullRequest(result.Message);
        repositoryLogger.LogInformation("Notify: " + result.Message);
    }

    public async Task ProcessPullRequestReviewApproveWebhookAsync(GithubPullRequestDescriptor pullRequestDescriptor, ILogger repositoryLogger, IPullRequetsEventNotifier pullRequestEventNotifier, string? commentBody)
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

        var getSubmissionCommand = new GetLastSubmissionByPr.Query(pullRequestDescriptor);

        var response = await _mediator.Send(getSubmissionCommand, CancellationToken.None);

        switch (response.Submission.Points)
        {
            case null:
                BaseShreksCommandResult result = await ProceedCommandAsync(approveCommand, pullRequestDescriptor, repositoryLogger);
                await pullRequestEventNotifier.SendCommentToPullRequest(result.Message);
                repositoryLogger.LogInformation("Notify: " + result.Message);
                break;

            case 100:
                {
                    var message = "Review successfully processed, but submission already has 100 points";
                    await pullRequestEventNotifier.SendCommentToPullRequest(message);
                    repositoryLogger.LogInformation("Notify: " + message);
                    break;
                }
            default:
                {
                    var message = $"Submission is already rated with {response.Submission.Points} points. If you want to change it, please use /update command.";
                    await pullRequestEventNotifier.SendCommentToPullRequest(message);
                    repositoryLogger.LogInformation("Notify: " + message);
                    break;
                }
        }
    }

    public async Task ProcessIssueCommentCreateWebhookAsync(
        GithubPullRequestDescriptor pullRequestDescriptor,
        ILogger repositoryLogger,
        IPullRequestCommentEventNotifier pullRequestCommentEventNotifier,
        string issueCommentBody)
    {
        if (issueCommentBody.FirstOrDefault() != '/')
            return;

        IShreksCommand command = _commandParser.Parse(issueCommentBody);
        var result = await ProceedCommandAsync(command, pullRequestDescriptor, repositoryLogger);
        if (!string.IsNullOrEmpty(result.Message))
            await pullRequestCommentEventNotifier.SendCommentToPullRequest(result.Message);

        await pullRequestCommentEventNotifier.ReactToUserComment(result.IsSuccess);
    }

    private async Task<BaseShreksCommandResult> ProceedCommandAsync(IShreksCommand command, GithubPullRequestDescriptor pullRequestDescriptor, ILogger repositoryLogger)
    {
        var contextCreator = new PullRequestCommentContextFactory(_mediator, pullRequestDescriptor, repositoryLogger);
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

    private async Task<Guid> GetSubjectCourseByOrganization(string organization, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetSubjectCourseByOrganization.Query(organization));
        return response.SubjectCourseId;
    }

    private async Task<Guid> GetUserByGithubLogin(string login, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetUserByGithubUsername.Query(login));
        return response.UserId;
    }

    private async Task<Guid> GetAssignmentByBranchAndSubjectCourse(string branch, Guid subjectCourseId)
    {
        var response = await _mediator.Send(new GetAssignmentByBranchAndSubjectCourse.Query(branch, subjectCourseId));
        return response.AssignmentId;
    }

    private async Task<SubmissionDto> GetGithubSubmissionAsync(string organization, string repository, long prNumber)
    {
        var query = new GetGithubSubmission.Query(organization, repository, prNumber);
        var response = await _mediator.Send(query);
        return response.Submission;
    }
}