using Kysect.Shreks.Core.Models;
using Kysect.Shreks.Core.Submissions;

namespace Kysect.Shreks.Core.Queue.Evaluators;

public class SubmissionDateTimeEvaluator : ISubmissionEvaluator
{
    public SubmissionDateTimeEvaluator(SortingOrder sortingOrder)
    {
        SortingOrder = sortingOrder;
    }

    public SortingOrder SortingOrder { get; }

    public double Evaluate(Submission submission)
        => submission.SubmissionDate.Value.Ticks;
}