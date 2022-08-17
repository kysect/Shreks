using Kysect.Shreks.Core.Models;
using Kysect.Shreks.Core.Queue.Visitors;
using Kysect.Shreks.Core.Study;
using RichEntity.Annotations;

namespace Kysect.Shreks.Core.Queue.Evaluators;

public abstract partial class SubmissionEvaluator : IEntity<Guid>, ISubmissionEvaluator
{
    protected SubmissionEvaluator(SortingOrder sortingOrder) : this(Guid.NewGuid())
    {
        SortingOrder = sortingOrder;
    }

    public SortingOrder SortingOrder { get; protected init; }

    public abstract ValueTask<T> AcceptAsync<T>(
        Submission submission,
        ISubmissionEvaluatorVisitor<T> visitor,
        CancellationToken cancellationToken);
}