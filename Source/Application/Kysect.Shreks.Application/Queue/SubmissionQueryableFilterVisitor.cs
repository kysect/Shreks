using Kysect.Shreks.Core.Queue.Filters;
using Kysect.Shreks.Core.Queue.Visitors;
using Kysect.Shreks.Core.Study;

namespace Kysect.Shreks.Application.Queue;

public class SubmissionQueryableFilterVisitor : IQueueFilterVisitor<IQueryable<Submission>>
{
    public ValueTask<IQueryable<Submission>> VisitAsync(
        IQueryable<Submission> value,
        GroupQueueFilter filter,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(filter);

        value = value.Where(x => filter.Groups.Contains(x.Student.Group));
        return ValueTask.FromResult(value);
    }
}