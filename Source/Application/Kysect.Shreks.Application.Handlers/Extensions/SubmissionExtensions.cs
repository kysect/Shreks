using Submission = Kysect.Shreks.Core.Submissions.Submission;

namespace Kysect.Shreks.Application.Handlers.Extensions;

internal static class SubmissionExtensions
{
    public static Guid GetCourseId(this Submission submission)
        => submission.GroupAssignment.Assignment.SubjectCourse.Id;
}