using Kysect.Shreks.Core.Models;
using Kysect.Shreks.Core.Submissions;

namespace Kysect.Shreks.Core.Queue.Evaluators;

public partial class SubmissionDateTimeEvaluator : SubmissionEvaluator
{
    public SubmissionDateTimeEvaluator(int position, SortingOrder sortingOrder) : base(position, sortingOrder) { }

    public override double Evaluate(Submission submission)
        => submission.SubmissionDate.Value.Ticks;
}