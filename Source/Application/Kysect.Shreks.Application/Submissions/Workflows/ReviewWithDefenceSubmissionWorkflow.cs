using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Application.Abstractions.Permissions;
using Kysect.Shreks.Application.Abstractions.Submissions;
using Kysect.Shreks.Application.Abstractions.Submissions.Models;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Dto.Submissions;
using Kysect.Shreks.Application.Extensions;
using Kysect.Shreks.Application.Factories;
using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Common.Resources;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Core.Submissions.States;
using Kysect.Shreks.Core.Tools;
using Kysect.Shreks.Core.ValueObject;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Kysect.Shreks.Application.Submissions.Workflows;

public class ReviewWithDefenceSubmissionWorkflow : ISubmissionWorkflow
{
    private readonly IShreksDatabaseContext _context;
    private readonly IPermissionValidator _permissionValidator;
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

        Submission submission = await ExecuteSubmissionCommandAsync(
            submissionId, cancellationToken, static x => x.Rate(0, 0));

        SubmissionRateDto submissionRateDto = SubmissionRateDtoFactory.CreateFromSubmission(submission);
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

        Submission submission = await ExecuteSubmissionCommandAsync(submissionId, cancellationToken, static x =>
        {
            if (x.Points is null)
                x.Rate(Fraction.FromDenormalizedValue(100), 0);
        });

        SubmissionRateDto submissionRateDto = SubmissionRateDtoFactory.CreateFromSubmission(submission);
        string message = UserCommandProcessingMessage.SubmissionRated(submissionRateDto.ToDisplayString());

        return new SubmissionActionMessageDto(message);
    }

    public async Task<SubmissionActionMessageDto> SubmissionRejectedAsync(
        Guid issuerId,
        Guid submissionId,
        CancellationToken cancellationToken)
    {
        await _permissionValidator.EnsureSubmissionMentorAsync(issuerId, submissionId, cancellationToken);

        Submission submission = await ExecuteSubmissionCommandAsync(
            submissionId, cancellationToken, static x => x.Deactivate());

        string message = UserCommandProcessingMessage.ClosePullRequestWithUnratedSubmission(submission.Code);

        return new SubmissionActionMessageDto(message);
    }

    public async Task<SubmissionActionMessageDto> SubmissionAbandonedAsync(
        Guid issuerId,
        Guid submissionId,
        bool isTerminal,
        CancellationToken cancellationToken)
    {
        Submission submission = await ExecuteSubmissionCommandAsync(submissionId, cancellationToken, x =>
        {
            if (isTerminal)
                x.Complete();
            else
                x.Deactivate();
        });

        string message = UserCommandProcessingMessage.MergePullRequestWithoutRate(submission.Code);
        return new SubmissionActionMessageDto(message);
    }

    public async Task<SubmissionUpdateResult> SubmissionUpdatedAsync(
        Guid issuerId,
        Guid userId,
        Guid assignmentId,
        ISubmissionFactory submissionFactory,
        CancellationToken cancellationToken)
    {
        ISubmissionState[] acceptedStates =
        {
            new ActiveSubmissionState(),
            new ReviewedSubmissionState(),
        };

        Submission? submission = await _context.Submissions
            .Where(x => x.Student.UserId.Equals(userId))
            .Where(x => x.GroupAssignment.Assignment.Id.Equals(assignmentId))
            .Where(submission => acceptedStates.Any(x => x.Equals(submission.State)))
            .OrderByDescending(x => x.Code)
            .FirstOrDefaultAsync(cancellationToken);

        bool triggeredByMentor = await _context.Assignments
            .Where(x => x.Id.Equals(assignmentId))
            .SelectMany(x => x.SubjectCourse.Mentors)
            .AnyAsync(x => x.UserId.Equals(issuerId), cancellationToken);

        bool triggeredByAnotherUser = issuerId.Equals(userId) is false;

        if (submission is null || submission.IsRated)
        {
            if (triggeredByAnotherUser && triggeredByMentor is false)
            {
                string message = $"User {issuerId} is not allowed to create new submission for user {userId}";
                throw new UnauthorizedException(message);
            }

            submission = await submissionFactory.CreateAsync(userId, assignmentId, cancellationToken);

            _context.Submissions.Add(submission);
            await _context.SaveChangesAsync(cancellationToken);

            await UpdatePointsSheetAsync(submission.Id, cancellationToken);

            SubmissionRateDto rateDto = SubmissionRateDtoFactory.CreateFromSubmission(submission);

            return new SubmissionUpdateResult(rateDto, true);
        }

        if (triggeredByMentor is false)
        {
            submission.UpdateDate(Calendar.CurrentDateTime);

            _context.Submissions.Update(submission);
            await _context.SaveChangesAsync(cancellationToken);

            await UpdatePointsSheetAsync(submission.Id, cancellationToken);

            if (triggeredByAnotherUser)
                throw new UnauthorizedException("Submission updated by another user");

            SubmissionRateDto submissionDto = SubmissionRateDtoFactory.CreateFromSubmission(submission);

            return new SubmissionUpdateResult(submissionDto, false);
        }

        // TODO: Proper mentor update handling
        SubmissionRateDto dto = SubmissionRateDtoFactory.CreateFromSubmission(submission);
        return new SubmissionUpdateResult(dto, false);
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

        _updateQueue.EnqueueCoursePointsUpdate(submission.GetSubjectCourseId());
        _updateQueue.EnqueueSubmissionsQueueUpdate(submission.GetSubjectCourseId(), submission.GetGroupId());

        return submission;
    }

    private async Task UpdatePointsSheetAsync(Guid submissionId, CancellationToken cancellationToken)
    {
        var submissionData = await _context.Submissions
            .Where(x => x.Id.Equals(submissionId))
            .Select(x => new { x.GroupAssignment.Assignment.SubjectCourse, x.Student.Group })
            .SingleOrDefaultAsync(cancellationToken);

        if (submissionData is { SubjectCourse: not null, Group: not null })
        {
            _updateQueue.EnqueueSubmissionsQueueUpdate(submissionData.SubjectCourse.Id, submissionData.Group.Id);
        }
    }
}