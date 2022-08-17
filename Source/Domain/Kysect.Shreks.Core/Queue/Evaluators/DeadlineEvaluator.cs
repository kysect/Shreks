using Kysect.Shreks.Core.Models;
using Kysect.Shreks.Core.Queue.Visitors;
using Kysect.Shreks.Core.Study;

namespace Kysect.Shreks.Core.Queue.Evaluators;

public partial class DeadlineEvaluator : SubmissionEvaluator
{
    public DeadlineEvaluator(SortingOrder sortingOrder) : base(sortingOrder) { }

    public override ValueTask<T> AcceptAsync<T>(
        Submission submission,
        ISubmissionEvaluatorVisitor<T> visitor,
        CancellationToken cancellationToken)
    {
        return visitor.VisitAsync(submission, this, cancellationToken);
    }
}