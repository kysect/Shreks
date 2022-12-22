using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Application.Abstractions.Permissions;
using Kysect.Shreks.Application.Abstractions.Submissions;
using Kysect.Shreks.Application.Dto.Submissions;
using Kysect.Shreks.Application.Extensions;
using Kysect.Shreks.Application.Factories;
using Kysect.Shreks.Common.Resources;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Core.ValueObject;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;

namespace Kysect.Shreks.Application.Submissions.Workflows;

public class ReviewWithDefenceSubmissionWorkflow : ISubmissionWorkflow
{
    private readonly IPermissionValidator _permissionValidator;
    private readonly IShreksDatabaseContext _context;
    private readonly ITableUpdateQueue _updateQueue;

    public ReviewWithDefenceSubmissionWorkflow(
        IPermissionValidator permissionValidator,
        IShreksDatabaseContext context,
        ITableUpdateQueue updateQueue)
    {
        _permissionValidator = permissionValidator;
        _context = context;
        _updateQueue = updateQueue;
    }

    public async Task<SubmissionActionMessageDto> SubmissionApprovedAsync(
        Guid issuerId,
        Guid submissionId,
        CancellationToken cancellationToken)
    {
        await _permissionValidator.EnsureSubmissionMentorAsync(issuerId, submissionId, cancellationToken);
        await ExecuteSubmissionCommandAsync(submissionId, cancellationToken, static x => x.MarkAsReviewed());

        string message = UserCommandProcessingMessage.SubmissionMarkAsReviewedAndNeedDefense();
        return new SubmissionActionMessageDto(message);
    }

    public async Task<SubmissionActionMessageDto> SubmissionNotAcceptedAsync(
        Guid issuerId,
        Guid submissionId,
        CancellationToken cancellationToken)
    {
        await _permissionValidator.EnsureSubmissionMentorAsync(issuerId, submissionId, cancellationToken);
        var submission = await ExecuteSubmissionCommandAsync(submissionId, cancellationToken, static x => x.Rate(0, 0));

        var submissionRateDto = SubmissionRateDtoFactory.CreateFromSubmission(submission);
        string message = UserCommandProcessingMessage.SubmissionMarkAsNotAccepted(submission.Code);

        message = $"{message}\n{submissionRateDto.ToDisplayString()}";

        return new SubmissionActionMessageDto(message);
    }

    public async Task<SubmissionActionMessageDto> SubmissionReactivatedAsync(
        Guid issuerId,
        Guid submissionId,
        CancellationToken cancellationToken)
    {
        await ExecuteSubmissionCommandAsync(submissionId, cancellationToken, static x => x.Activate());

        string message = UserCommandProcessingMessage.SubmissionActivatedSuccessfully();
        return new SubmissionActionMessageDto(message);
    }

    public async Task<SubmissionActionMessageDto> SubmissionAcceptedAsync(
        Guid issuerId,
        Guid submissionId,
        CancellationToken cancellationToken)
    {
        await _permissionValidator.EnsureSubmissionMentorAsync(issuerId, submissionId, cancellationToken);

        var submission = await ExecuteSubmissionCommandAsync(submissionId, cancellationToken, static x =>
        {
            if (x.Points is null)
                x.Rate(Fraction.FromDenormalizedValue(100), 0);
        });

        var submissionRateDto = SubmissionRateDtoFactory.CreateFromSubmission(submission);
        string message = UserCommandProcessingMessage.SubmissionMarkAsNotAccepted(submission.Code);

        message = $"{message}\n{submissionRateDto.ToDisplayString()}";

        return new SubmissionActionMessageDto(message);
    }

    public async Task<SubmissionActionMessageDto> SubmissionRejectedAsync(
        Guid issuerId,
        Guid submissionId,
        CancellationToken cancellationToken)
    {
        await _permissionValidator.EnsureSubmissionMentorAsync(issuerId, submissionId, cancellationToken);

        var submission = await ExecuteSubmissionCommandAsync(
            submissionId, cancellationToken, static x => x.Deactivate());

        var message = UserCommandProcessingMessage.ClosePullRequestWithUnratedSubmission(submission.Code);

        return new SubmissionActionMessageDto(message);
    }

    public async Task<SubmissionActionMessageDto> SubmissionAbandonedAsync(
        Guid issuerId,
        Guid submissionId,
        bool isTerminal,
        CancellationToken cancellationToken)
    {
        var submission = await ExecuteSubmissionCommandAsync(submissionId, cancellationToken, x =>
        {
            if (isTerminal)
            {
                x.Complete();
            }
            else
            {
                x.Deactivate();
            }
        });

        string message = UserCommandProcessingMessage.MergePullRequestWithoutRate(submission.Code);
        return new SubmissionActionMessageDto(message);
    }

    private async Task<Submission> ExecuteSubmissionCommandAsync(
        Guid submissionId,
        CancellationToken cancellationToken,
        Action<Submission> action)
    {
        Submission submission = await _context.Submissions.GetByIdAsync(submissionId, cancellationToken);
        action(submission);

        _context.Submissions.Update(submission);
        await _context.SaveChangesAsync(cancellationToken);

        _updateQueue.EnqueueCoursePointsUpdate(submission.GetCourseId());
        _updateQueue.EnqueueSubmissionsQueueUpdate(submission.GetCourseId(), submission.GetGroupId());

        return submission;
    }
}