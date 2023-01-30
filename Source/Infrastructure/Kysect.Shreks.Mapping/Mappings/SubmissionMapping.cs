using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Dto.Tables;
using Kysect.Shreks.Core.Submissions;

namespace Kysect.Shreks.Mapping.Mappings;

public static class SubmissionMapping
{
    public static SubmissionDto ToDto(this Submission submission)
    {
        return new SubmissionDto(
            submission.Id,
            submission.Code,
            submission.SubmissionDate.AsDateTime(),
            submission.Student.UserId,
            submission.GroupAssignment.AssignmentId,
            submission.Payload,
            submission.ExtraPoints.AsDto(),
            submission.Points.AsDto(),
            submission.GroupAssignment.Assignment.ShortName,
            submission.State.Kind.AsDto());
    }

    public static QueueSubmissionDto ToQueueDto(this Submission submission)
    {
        return new QueueSubmissionDto(submission.Student.ToDto(), submission.ToDto());
    }
}