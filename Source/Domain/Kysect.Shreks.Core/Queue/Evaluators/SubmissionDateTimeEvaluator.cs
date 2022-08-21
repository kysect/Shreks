using Kysect.Shreks.Core.Models;
using Submission = Kysect.Shreks.Core.Submissions.Submission;

namespace Kysect.Shreks.Core.Queue.Evaluators;

public partial class SubmissionDateTimeEvaluator : SubmissionEvaluator
{
    public SubmissionDateTimeEvaluator(int position, SortingOrder sortingOrder) : base(position, sortingOrder) { }

    public override double Evaluate(Submission submission)
        => submission.SubmissionDate.ToDateTime(TimeOnly.FromTimeSpan(TimeSpan.Zero)).Ticks;
}