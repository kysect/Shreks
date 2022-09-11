using Kysect.Shreks.Core.Models;
using Kysect.Shreks.Core.Submissions;

namespace Kysect.Shreks.Core.Queue.Evaluators;

public class SubmissionStateEvaluator : ISubmissionEvaluator
{
    public SubmissionStateEvaluator(SortingOrder sortingOrder)
    {
        SortingOrder = sortingOrder;
    }

    public SortingOrder SortingOrder { get; }

    public double Evaluate(Submission submission)
    {
        return submission.State switch
        {
            SubmissionState.Active => 2,
            SubmissionState.Reviewed => 1,
            _ => 0,
        };
    }
}