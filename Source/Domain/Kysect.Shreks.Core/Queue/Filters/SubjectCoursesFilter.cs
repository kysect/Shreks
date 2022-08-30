using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Submissions;

namespace Kysect.Shreks.Core.Queue.Filters;

public partial class SubjectCoursesFilter : SubmissionQueueFilter
{
    public SubjectCoursesFilter(Guid courseId) : base(Guid.NewGuid())
    {
        CourseId = courseId;
    }

    public Guid CourseId { get; protected init; }

    public override IQueryable<Submission> Filter(IQueryable<Submission> query)
    {
        return query.Where(x => CourseId.Equals(x.GroupAssignment.Assignment.SubjectCourse.Id));
    }
}