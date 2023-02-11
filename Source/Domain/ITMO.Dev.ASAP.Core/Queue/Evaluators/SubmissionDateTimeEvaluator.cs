using ITMO.Dev.ASAP.Core.Models;
using ITMO.Dev.ASAP.Core.Submissions;

namespace ITMO.Dev.ASAP.Core.Queue.Evaluators;

public class SubmissionDateTimeEvaluator : ISubmissionEvaluator
{
    public SubmissionDateTimeEvaluator(SortingOrder sortingOrder)
    {
        SortingOrder = sortingOrder;
    }

    public SortingOrder SortingOrder { get; }

    public double Evaluate(Submission submission)
    {
        return submission.SubmissionDate.Value.Ticks;
    }
}