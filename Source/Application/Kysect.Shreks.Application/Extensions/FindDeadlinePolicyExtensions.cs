using Kysect.Shreks.Core.DeadlinePolicies;
using Kysect.Shreks.Core.Submissions;

namespace Kysect.Shreks.Application.Extensions
{
    public static class FindDeadlinePolicyExtensions
    {
        public static DeadlinePolicy? FindActiveDeadlinePolicy(this Submission submission, DateOnly deadline)
        {
            ArgumentNullException.ThrowIfNull(submission);

            if (submission.SubmissionDateOnly <= deadline)
                return null;

            TimeSpan submissionDeadlineOffset = TimeSpan.FromDays(submission.SubmissionDateOnly.DayNumber - deadline.DayNumber);
            DeadlinePolicy? activeDeadlinePolicy = submission
                .GroupAssignment
                .Assignment
                .SubjectCourse
                .DeadlinePolicies
                .Where(dp => dp.SpanBeforeActivation < submissionDeadlineOffset)
                .MaxBy(dp => dp.SpanBeforeActivation);


            return activeDeadlinePolicy;
        }
    }
}