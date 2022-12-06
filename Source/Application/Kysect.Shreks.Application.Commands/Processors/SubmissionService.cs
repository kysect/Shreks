using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Application.Extensions;
using Kysect.Shreks.Application.Validators;
using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Core.Tools;
using Kysect.Shreks.Core.ValueObject;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Kysect.Shreks.Application.Commands.Processors;

public class SubmissionService : ISubmissionService
{
    private readonly IShreksDatabaseContext _context;
    private readonly ITableUpdateQueue _updateQueue;

    public SubmissionService(IShreksDatabaseContext context, ITableUpdateQueue updateQueue)
    {
        _context = context;
        _updateQueue = updateQueue;
    }

    public async Task<Submission> GetSubmissionByCodeAsync(
        int code,
        Guid studentId,
        Guid assignmentId,
        CancellationToken cancellationToken)
    {
        Submission? submission = await _context.Submissions
            .Where(s => s.Code == code)
            .Where(s => s.Student.UserId == studentId)
            .Where(s => s.GroupAssignment.AssignmentId == assignmentId)
            .FirstOrDefaultAsync(cancellationToken);

        if (submission is null)
            throw new EntityNotFoundException($"Couldn't find submission with code ${code}");

        return submission;
    }

    public Task<Submission> UpdateSubmissionDate(
        Guid submissionId,
        Guid userId,
        DateOnly newDate,
        CancellationToken cancellationToken)
    {
        return ExecuteSubmissionCommandAsync(userId, submissionId, cancellationToken, x =>
        {
            x.UpdateDate(SpbDateTime.FromDateOnly(newDate));
        });
    }

    public Task<Submission> UpdateSubmissionPoints(
        Guid submissionId,
        Guid userId,
        double? newRating,
        double? extraPoints,
        CancellationToken cancellationToken)
    {
        return ExecuteSubmissionCommandAsync(userId, submissionId, cancellationToken, x =>
        {
            Fraction? fraction = newRating is null ? null : new Fraction(newRating.Value / 100);
            Points? extraPointsTyped = extraPoints is null ? null : new Points(extraPoints.Value);

            x.Rate(fraction, extraPointsTyped);
        });
    }

    public Task<Submission> CompleteSubmissionAsync(Guid submissionId, Guid userId, CancellationToken cancellationToken)
    {
        return ExecuteSubmissionCommandAsync(userId, submissionId, cancellationToken, x => x.Complete(), false);
    }

    public Task<Submission> ActivateSubmissionAsync(Guid submissionId, Guid userId, CancellationToken cancellationToken)
    {
        return ExecuteSubmissionCommandAsync(userId, submissionId, cancellationToken, x => x.Activate(), false);
    }

    public Task<Submission> DeactivateSubmissionAsync(
        Guid submissionId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        return ExecuteSubmissionCommandAsync(userId, submissionId, cancellationToken, x => x.Deactivate(), false);
    }

    public Task<Submission> DeleteSubmissionAsync(Guid submissionId, Guid userId, CancellationToken cancellationToken)
    {
        return ExecuteSubmissionCommandAsync(userId, submissionId, cancellationToken, x => x.Delete());
    }

    public Task<Submission> ReviewSubmissionAsync(Guid submissionId, Guid userId, CancellationToken cancellationToken)
    {
        return ExecuteSubmissionCommandAsync(userId, submissionId, cancellationToken, x => x.MarkAsReviewed());
    }

    public Task<Submission> BanSubmissionAsync(Guid submissionId, Guid userId, CancellationToken cancellationToken)
    {
        return ExecuteSubmissionCommandAsync(userId, submissionId, cancellationToken, static x => x.Ban());
    }

    private async Task<Submission> ExecuteSubmissionCommandAsync(
        Guid userId,
        Guid submissionId,
        CancellationToken cancellationToken,
        Action<Submission> action,
        bool mustHaveMentorAccess = true)
    {
        Submission submission = await _context.Submissions.GetByIdAsync(submissionId, cancellationToken);

        if (mustHaveMentorAccess)
            PermissionValidator.EnsureMentorAccess(userId, submission);

        action(submission);

        _context.Submissions.Update(submission);
        await _context.SaveChangesAsync(cancellationToken);

        _updateQueue.EnqueueCoursePointsUpdate(submission.GetCourseId());
        _updateQueue.EnqueueSubmissionsQueueUpdate(submission.GetCourseId(), submission.GetGroupId());

        return submission;
    }
}