using Kysect.Shreks.Core.Models;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Core.Tools;

namespace Kysect.Shreks.Core.Queue.Evaluators;

public class AssignmentDeadlineStateEvaluator : ISubmissionEvaluator
{
    private const double CurrentAssignmentPriority = 3;
    private const double ProperlySubmittedAssignmentPriority = 2;
    private const double ExpiredAssignmentPriority = 1;
    private const double OtherAssignmentPriority = 0;

    public AssignmentDeadlineStateEvaluator(SortingOrder sortingOrder)
    {
        SortingOrder = sortingOrder;
    }

    public SortingOrder SortingOrder { get; }

    public double Evaluate(Submission submission)
    {
        ArgumentNullException.ThrowIfNull(submission);

        var groupAssignment = submission.GroupAssignment;

        if (groupAssignment.Deadline < submission.SubmissionDateOnly)
            return ExpiredAssignmentPriority;

        var now = Calendar.CurrentDate;

        var closestDeadline = submission
            .GroupAssignment
            .Assignment
            .SubjectCourse
            .Assignments
            .SelectMany(x => x.GroupAssignments)
            .Where(x => x.Group.Equals(submission.Student.Group))
            .Select(x => x.Deadline)
            .Where(x => x >= now)
            .Min();

        if (groupAssignment.Deadline.Equals(closestDeadline))
            return CurrentAssignmentPriority;

        if (groupAssignment.Deadline >= submission.SubmissionDateOnly)
            return ProperlySubmittedAssignmentPriority;

        return OtherAssignmentPriority;
    }
}