using Kysect.Shreks.Core.Models;
using Kysect.Shreks.Core.Submissions;

namespace Kysect.Shreks.Core.Queue.Filters;

public class SubmissionStateFilter : IQueueFilter
{
    public SubmissionStateFilter(IReadOnlyCollection<SubmissionState> states)
    {
        States = states;
    }

    public SubmissionStateFilter(params SubmissionState[] states)
        : this((IReadOnlyCollection<SubmissionState>)states) { }

    public IReadOnlyCollection<SubmissionState> States { get; }

    public IQueryable<Submission> Filter(IQueryable<Submission> query)
        => query.Where(x => States.Contains(x.State));
}