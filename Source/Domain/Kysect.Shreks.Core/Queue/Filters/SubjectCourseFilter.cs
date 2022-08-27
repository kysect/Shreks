using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Submissions;

namespace Kysect.Shreks.Core.Queue.Filters;

public partial class SubjectCourseFilter : SubmissionQueueFilter
{
    public SubjectCourseFilter(IReadOnlyCollection<SubjectCourse> courses) : this(Guid.NewGuid())
    {
        Courses = courses;
    }

    public virtual IReadOnlyCollection<SubjectCourse> Courses { get; protected init; }

    public override IQueryable<Submission> Filter(IQueryable<Submission> query)
    {
        return query.Where(x => Courses.Any(xx => xx.Equals(x.GroupAssignment.Assignment.SubjectCourse)));
    }
}