using Kysect.Shreks.Core.Models;
using Kysect.Shreks.Core.Study;

namespace Kysect.Shreks.Core.Queue.Evaluators;

public partial class DeadlineEvaluator : SubmissionEvaluator
{
    public DeadlineEvaluator(int position, SortingOrder sortingOrder) : base(position, sortingOrder) { }

    public override double Evaluate(Submission submission)
    {
        ArgumentNullException.ThrowIfNull(submission);

        var groupAssignment = submission.Assignment
            .GroupAssignments
            .Single(x => x.Group.Equals(submission.Student.Group));
            
        return groupAssignment.Deadline > submission.SubmissionDateTime ? 1 : 0;
    }
}