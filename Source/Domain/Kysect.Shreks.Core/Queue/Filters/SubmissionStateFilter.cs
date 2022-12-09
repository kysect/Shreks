using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Core.Submissions.States;

namespace Kysect.Shreks.Core.Queue.Filters;

public class SubmissionStateFilter : IQueueFilter
{
    public SubmissionStateFilter(IReadOnlyCollection<ISubmissionState> states)
    {
        States = states;
    }

    public SubmissionStateFilter(params ISubmissionState[] states)
        : this((IReadOnlyCollection<ISubmissionState>)states) { }

    public IReadOnlyCollection<ISubmissionState> States { get; }

    public IQueryable<Submission> Filter(IQueryable<Submission> query)
        => query.Where(x => States.Any(xx => xx.Equals(x.State)));
}