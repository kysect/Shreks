using Kysect.Shreks.Core.Queue.Visitors;
using RichEntity.Annotations;

namespace Kysect.Shreks.Core.Queue.Filters;

public abstract partial class QueueFilter : IEntity<Guid>, IQueueFilter
{
    protected virtual SubmissionQueue SubmissionQueue { get; init; }

    public abstract ValueTask<T> AcceptAsync<T>(
        T value,
        IQueueFilterVisitor<T> visitor,
        CancellationToken cancellationToken);
}