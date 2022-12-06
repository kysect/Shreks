using Kysect.Shreks.Core.Models;
using Kysect.Shreks.Core.Submissions;

namespace Kysect.Shreks.Core.Queue.Filters;

public class SubmissionStateFilter : IQueueFilter
{
    public SubmissionStateFilter(IReadOnlyCollection<SubmissionStateKind> states)
    {
        States = states;
    }

    public SubmissionStateFilter(params SubmissionStateKind[] states)
        : this((IReadOnlyCollection<SubmissionStateKind>)states) { }

    public IReadOnlyCollection<SubmissionStateKind> States { get; }

    public IQueryable<Submission> Filter(IQueryable<Submission> query)
        => query.Where(x => States.Any(xx => xx.Equals(x.State)));
}