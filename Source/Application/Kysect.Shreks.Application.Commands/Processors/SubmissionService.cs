using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Application.Extensions;
using Kysect.Shreks.Application.Validators;
using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.Models;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Core.Tools;
using Kysect.Shreks.Core.ValueObject;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;

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

    public async Task<Submission> UpdateSubmissionState(Guid submissionId, Guid userId, SubmissionState state, CancellationToken cancellationToken)
    {
        Submission submission = await _context.Submissions.GetByIdAsync(submissionId, cancellationToken);

        PermissionValidator.EnsureHasAccessToRepository(userId, submission);

        submission.State = state;
        await _context.SaveChangesAsync(cancellationToken);

        _updateQueue.EnqueueSubmissionsQueueUpdate(submission.GetCourseId(), submission.GetGroupId());

        return submission;
    }

    public async Task<Submission> UpdateSubmissionDate(Guid submissionId, Guid userId, DateOnly newDate, CancellationToken cancellationToken)
    {
        Submission submission = await _context.Submissions.GetByIdAsync(submissionId, cancellationToken);

        if (!PermissionValidator.IsRepositoryMentor(userId, submission))
            throw new UnauthorizedException("Only mentors can change submission date");

        submission.SubmissionDate = SpbDateTime.FromDateOnly(newDate);
        _context.Submissions.Update(submission);
        await _context.SaveChangesAsync(cancellationToken);

        _updateQueue.EnqueueSubmissionsQueueUpdate(submission.GetCourseId(), submission.GetGroupId());
        _updateQueue.EnqueueCoursePointsUpdate(submission.GetCourseId());

        return submission;
    }

    public async Task<Submission> UpdateSubmissionPoints(Guid submissionId, Guid userId, double? newRating, double? extraPoints, CancellationToken cancellationToken)
    {
        Submission submission = await _context.Submissions.GetByIdAsync(submissionId, cancellationToken);

        if (!PermissionValidator.IsRepositoryMentor(userId, submission))
            throw new UnauthorizedException("Only mentors can rate submission");

        Fraction? fraction = newRating is null ? null : new Fraction(newRating.Value / 100);
        Points? extraPointsTyped = extraPoints is null ? null : new Points(extraPoints.Value);

        submission.Rate(fraction, extraPointsTyped);

        _context.Submissions.Update(submission);
        await _context.SaveChangesAsync(cancellationToken);

        _updateQueue.EnqueueCoursePointsUpdate(submission.GetCourseId());
        _updateQueue.EnqueueSubmissionsQueueUpdate(submission.GetCourseId(), submission.GetGroupId());

        return submission;
    }
}