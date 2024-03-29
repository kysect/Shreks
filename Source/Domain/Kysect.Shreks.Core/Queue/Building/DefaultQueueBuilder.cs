using Kysect.Shreks.Core.Models;
using Kysect.Shreks.Core.Queue.Evaluators;
using Kysect.Shreks.Core.Queue.Filters;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Submissions.States;

namespace Kysect.Shreks.Core.Queue.Building;

public class DefaultQueueBuilder : QueueBuilder
{
    public DefaultQueueBuilder(StudentGroup group, Guid subjectCourseId)
    {
        AddFilter(new GroupQueueFilter(new[] { group }));
        AddFilter(new SubmissionStateFilter(new ActiveSubmissionState(), new ReviewedSubmissionState()));
        AddFilter(new SubjectCoursesFilter(subjectCourseId));

        AddEvaluator(new SubmissionStateEvaluator(SortingOrder.Descending));
        AddEvaluator(new AssignmentDeadlineStateEvaluator(SortingOrder.Descending));
        AddEvaluator(new SubmissionDateTimeEvaluator(SortingOrder.Ascending));
    }
}