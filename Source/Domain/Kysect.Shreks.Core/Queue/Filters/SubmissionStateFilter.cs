using Kysect.Shreks.Core.Models;
using Kysect.Shreks.Core.Submissions;

namespace Kysect.Shreks.Core.Queue.Filters;

public partial class SubmissionStateFilter : SubmissionQueueFilter
{
    public SubmissionStateFilter(SubmissionState state) : this(Guid.NewGuid())
    {
        State = state;
    }

    public SubmissionState State { get; protected init; }

    public override IQueryable<Submission> Filter(IQueryable<Submission> query)
    {
        return query.Where(x => x.State.Equals(State));
    }
}