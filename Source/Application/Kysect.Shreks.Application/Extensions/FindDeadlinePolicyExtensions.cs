using Kysect.Shreks.Core.DeadlinePolicies;
using Kysect.Shreks.Core.Submissions;

namespace Kysect.Shreks.Application.Extensions
{
    public static class FindDeadlinePolicyExtensions
    {
        public static DeadlinePolicy? FindActiveDeadlinePolicy(this Submission submission, DateOnly deadline)
        {
            ArgumentNullException.ThrowIfNull(submission);

            if (submission.SubmissionDate <= deadline)
                return null;

            var submissionDeadlineOffset = TimeSpan.FromDays(submission.SubmissionDate.DayNumber - deadline.DayNumber);
            return submission
                .GroupAssignment
                .Assignment
                .SubjectCourse
                .DeadlinePolicies
                .Where(dp => dp.SpanBeforeActivation < submissionDeadlineOffset)
                .MaxBy(dp => dp.SpanBeforeActivation);
        }
    }
}