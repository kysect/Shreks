using Kysect.Shreks.Core.Queue.Filters;

namespace Kysect.Shreks.Core.Queue.Visitors;

public interface IQueueFilterVisitor<T>
{
    ValueTask<T> VisitAsync(T value, GroupQueueFilter filter, CancellationToken cancellationToken);
}