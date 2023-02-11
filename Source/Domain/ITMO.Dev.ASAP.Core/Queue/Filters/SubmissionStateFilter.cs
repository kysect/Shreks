using ITMO.Dev.ASAP.Core.Submissions;
using ITMO.Dev.ASAP.Core.Submissions.States;

namespace ITMO.Dev.ASAP.Core.Queue.Filters;

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
    {
        return query.Where(x => States.Any(xx => xx.Equals(x.State)));
    }
}