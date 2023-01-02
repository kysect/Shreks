using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Submissions;

namespace Kysect.Shreks.Core.Queue.Filters;

public class AssignmentGroupFilter : IQueueFilter
{
    public AssignmentGroupFilter(IReadOnlyCollection<Assignment> assignments)
    {
        Assignments = assignments;
    }

    public IReadOnlyCollection<Assignment> Assignments { get; }

    public IQueryable<Submission> Filter(IQueryable<Submission> query)
    {
        return query.Where(x => Assignments.Contains(x.GroupAssignment.Assignment));
    }
}