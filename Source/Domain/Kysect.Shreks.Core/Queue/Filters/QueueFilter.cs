using Kysect.Shreks.Core.Submissions;
using RichEntity.Annotations;

namespace Kysect.Shreks.Core.Queue.Filters;

public abstract partial class QueueFilter : IEntity<Guid>, IQueueFilter
{
    public abstract IQueryable<Submission> Filter(IQueryable<Submission> query);
}