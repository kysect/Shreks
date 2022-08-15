using Kysect.Shreks.Core.Study;

namespace Kysect.Shreks.Application.Dto.Study;

public record SubmissionDto(
    Guid Id,
    DateTime SubmissionDateTime,
    Guid StudentId,
    Guid AssignmentId,
    string Payload,
    double ExtraPoints,
    double Points);

public static class SubmissionExtensions
{
    public static SubmissionDto ToDto(this Submission submission)
    {
        return new SubmissionDto(
            submission.Id,
            submission.SubmissionDateTime,
            submission.Student.Id,
            submission.Assignment.Id,
            submission.Payload,
            submission.ExtraPoints,
            submission.Points);
    }
}