using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.Core.Submissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace ITMO.Dev.ASAP.Application.Extensions;

public static class SubmissionExtensions
{
    public static Guid GetSubjectCourseId(this Submission submission)
    {
        return submission.GroupAssignment.Assignment.SubjectCourse.Id;
    }

    public static Guid GetGroupId(this Submission submission)
    {
        return submission.GroupAssignment.GroupId;
    }

    public static IIncludableQueryable<Submission, SubjectCourse> IncludeSubjectCourse(
        this IQueryable<Submission> query)
    {
        return query.Include(x => x.GroupAssignment.Assignment.SubjectCourse);
    }

    public static IIncludableQueryable<Submission, StudentGroup?> IncludeStudentGroup(this IQueryable<Submission> query)
    {
        return query.Include(x => x.Student.Group);
    }
}