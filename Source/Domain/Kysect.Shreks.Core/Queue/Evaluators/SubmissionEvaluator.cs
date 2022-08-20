using Kysect.Shreks.Core.Models;
using Kysect.Shreks.Core.Study;
using RichEntity.Annotations;

namespace Kysect.Shreks.Core.Queue.Evaluators;

public abstract partial class SubmissionEvaluator : IEntity<Guid>, ISubmissionEvaluator
{
    protected SubmissionEvaluator(int position, SortingOrder sortingOrder) : this(Guid.NewGuid())
    {
        Position = position;
        SortingOrder = sortingOrder;
    }

    public int Position { get; protected init; }
    public SortingOrder SortingOrder { get; protected init; }

    public abstract double Evaluate(Submission submission);
}