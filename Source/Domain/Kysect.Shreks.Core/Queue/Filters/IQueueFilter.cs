using Kysect.Shreks.Core.Queue.Visitors;

namespace Kysect.Shreks.Core.Queue.Filters;

public interface IQueueFilter
{
    ValueTask<T> AcceptAsync<T>(T value, IQueueFilterVisitor<T> visitor, CancellationToken cancellationToken);
}