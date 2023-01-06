using Kysect.Shreks.Core.Models;
using Kysect.Shreks.Core.Submissions;

namespace Kysect.Shreks.Core.Queue.Evaluators;

public class SubmissionStateEvaluator : ISubmissionEvaluator
{
    private const double ActiveSubmissionPriority = 2;
    private const double ReviewedSubmissionPriority = 1;

    public SubmissionStateEvaluator(SortingOrder sortingOrder)
    {
        SortingOrder = sortingOrder;
    }

    public SortingOrder SortingOrder { get; }

    public double Evaluate(Submission submission)
    {
        ArgumentNullException.ThrowIfNull(submission);

        return submission.State.Kind switch
        {
            SubmissionStateKind.Active => ActiveSubmissionPriority,
            SubmissionStateKind.Reviewed => ReviewedSubmissionPriority,
            SubmissionStateKind.Inactive => 0D,
            SubmissionStateKind.Deleted => 0D,
            SubmissionStateKind.Completed => 0D,
            SubmissionStateKind.Banned => 0D,
            _ => 0D,
        };
    }
}