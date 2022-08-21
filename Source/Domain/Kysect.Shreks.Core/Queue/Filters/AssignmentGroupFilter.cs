using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Submissions;

namespace Kysect.Shreks.Core.Queue.Filters;

public partial class AssignmentGroupFilter : QueueFilter
{
    public virtual IReadOnlyCollection<Assignment> Assignments { get; protected init; }

    public override IQueryable<Submission> Filter(IQueryable<Submission> query)
        => query.Where(x => Assignments.Contains(x.GroupAssignment.Assignment));
}