using ITMO.Dev.ASAP.Core.Submissions;

namespace ITMO.Dev.ASAP.Core.Queue.Filters;

public class SubjectCoursesFilter : IQueueFilter
{
    public SubjectCoursesFilter(Guid courseId)
    {
        CourseId = courseId;
    }

    public Guid CourseId { get; }

    public IQueryable<Submission> Filter(IQueryable<Submission> query)
    {
        return query.Where(x => CourseId.Equals(x.GroupAssignment.Assignment.SubjectCourse.Id));
    }
}