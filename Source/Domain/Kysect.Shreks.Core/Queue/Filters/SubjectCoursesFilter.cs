using Kysect.Shreks.Core.Submissions;

namespace Kysect.Shreks.Core.Queue.Filters;

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