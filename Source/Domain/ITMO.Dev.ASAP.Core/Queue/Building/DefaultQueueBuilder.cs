using ITMO.Dev.ASAP.Core.Models;
using ITMO.Dev.ASAP.Core.Queue.Evaluators;
using ITMO.Dev.ASAP.Core.Queue.Filters;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.Core.Submissions.States;

namespace ITMO.Dev.ASAP.Core.Queue.Building;

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