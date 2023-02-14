using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Application.Dto.Tables;
using ITMO.Dev.ASAP.Core.Submissions;

namespace ITMO.Dev.ASAP.Mapping.Mappings;

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