using Kysect.Shreks.Core.DeadlinePolicies;
using Kysect.Shreks.Core.Submissions;

namespace Kysect.Shreks.Application
{
    public static class FindDeadlinePolicySpecification
    {
        public static DeadlinePolicy? GetActiveDeadlinePolicy(this Submission submission, DateOnly deadline)
        {
            if (submission.SubmissionDate <= deadline)
                return null;

            var submissionDeadlineOffset = TimeSpan.FromDays(submission.SubmissionDate.DayNumber - deadline.DayNumber);
            return submission
                .GroupAssignment
                .Assignment
                .DeadlinePolicies
                .Where(dp => dp.SpanBeforeActivation < submissionDeadlineOffset)
                .MaxBy(dp => dp.SpanBeforeActivation);
        }
    }
}