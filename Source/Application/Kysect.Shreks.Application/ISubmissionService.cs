using Kysect.Shreks.Core.Submissions;

namespace Kysect.Shreks.Application;

public interface ISubmissionService
{
    Task<Submission> RateSubmissionAsync(
        Guid submissionId,
        Guid userId,
        double? newRating,
        double? extraPoints,
        CancellationToken cancellationToken);

    Task<Submission> GetSubmissionByCodeAsync(
        int code,
        Guid studentId,
        Guid assignmentId,
        CancellationToken cancellationToken);

    Task<Submission> UpdateSubmissionDate(
        Guid submissionId,
        Guid userId,
        DateOnly newDate,
        CancellationToken cancellationToken);

    Task<Submission> UpdateSubmissionPoints(
        Guid submissionId,
        Guid userId,
        double? newRating,
        double? extraPoints,
        CancellationToken cancellationToken);

    Task<Submission> CompleteSubmissionAsync(
        Guid submissionId,
        Guid userId,
        CancellationToken cancellationToken);

    Task<Submission> ActivateSubmissionAsync(
        Guid submissionId,
        Guid userId,
        CancellationToken cancellationToken);

    Task<Submission> DeactivateSubmissionAsync(
        Guid submissionId,
        Guid userId,
        CancellationToken cancellationToken);

    Task<Submission> DeleteSubmissionAsync(
        Guid submissionId,
        Guid userId,
        CancellationToken cancellationToken);

    Task<Submission> ReviewSubmissionAsync(
        Guid submissionId,
        Guid userId,
        CancellationToken cancellationToken);

    Task<Submission> BanSubmissionAsync(Guid submissionId, Guid userId, CancellationToken cancellationToken);
}