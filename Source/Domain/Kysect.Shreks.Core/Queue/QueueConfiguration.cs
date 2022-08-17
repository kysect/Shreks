using Kysect.Shreks.Core.Queue.Evaluators;
using Kysect.Shreks.Core.Queue.Filters;
using RichEntity.Annotations;

namespace Kysect.Shreks.Core.Queue;

public partial class QueueConfiguration : IEntity<Guid>
{
    public QueueConfiguration(
        IReadOnlyCollection<QueueFilter> filters,
        IReadOnlyList<SubmissionEvaluator> evaluators)
        : this(Guid.NewGuid())
    {
        Filters = filters;
        Evaluators = evaluators;
    }

    public virtual IReadOnlyCollection<QueueFilter> Filters { get; protected init; }

    public virtual IReadOnlyList<SubmissionEvaluator> Evaluators { get; protected init; }
}