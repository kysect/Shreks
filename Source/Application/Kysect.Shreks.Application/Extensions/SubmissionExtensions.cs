using Kysect.Shreks.Core.Submissions;

namespace Kysect.Shreks.Application.Extensions;

public static class SubmissionExtensions
{
    public static Guid GetCourseId(this Submission submission)
    {
        return submission.GroupAssignment.Assignment.SubjectCourse.Id;
    }

    public static Guid GetGroupId(this Submission submission)
    {
        return submission.GroupAssignment.GroupId;
    }
}