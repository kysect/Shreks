using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.Core.Submissions;

namespace ITMO.Dev.ASAP.Core.Queue.Filters;

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