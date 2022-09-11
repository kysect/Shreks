using Kysect.Shreks.Core.Models;
using Kysect.Shreks.Core.Submissions;

namespace Kysect.Shreks.Core.Queue.Evaluators;

public partial class SubmissionStateEvaluator : SubmissionEvaluator
{
    public SubmissionStateEvaluator(int position, SortingOrder sortingOrder) : base(position, sortingOrder) { }

    public override double Evaluate(Submission submission)
    {
        return submission.State switch
        {
            SubmissionState.Active => 2,
            SubmissionState.Reviewed => 1,
            _ => 0,
        };
    }
}