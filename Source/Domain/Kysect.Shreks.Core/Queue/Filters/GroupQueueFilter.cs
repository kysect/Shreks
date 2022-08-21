using Kysect.Shreks.Core.Study;
using Submission = Kysect.Shreks.Core.Submissions.Submission;

namespace Kysect.Shreks.Core.Queue.Filters;

public partial class GroupQueueFilter : QueueFilter
{
    protected GroupQueueFilter(IReadOnlyCollection<StudentGroup> groups)
    {
        Groups = groups;
    }

    public virtual IReadOnlyCollection<StudentGroup> Groups { get; protected init; }

    public override IQueryable<Submission> Filter(IQueryable<Submission> query)
        => query.Where(s => Groups.Contains(s.Student.Group));
}