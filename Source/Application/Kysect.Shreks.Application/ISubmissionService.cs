using Kysect.Shreks.Core.Models;
using Kysect.Shreks.Core.Submissions;

namespace Kysect.Shreks.Application;

public interface ISubmissionService
{
    Task<Submission> UpdateSubmissionState(Guid submissionId, Guid userId, SubmissionState state, CancellationToken cancellationToken);
    Task<Submission> UpdateSubmissionDate(Guid submissionId, Guid userId, DateOnly newDate, CancellationToken cancellationToken);
    Task<Submission> UpdateSubmissionPoints(Guid submissionId, Guid userId, double? newRating, double? extraPoints, CancellationToken cancellationToken);
    Task<Submission> GetSubmissionByCode(int code, Guid studentId, Guid assignmentId, CancellationToken cancellationToken);
}