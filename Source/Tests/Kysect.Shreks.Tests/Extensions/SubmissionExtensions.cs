using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Core.Users;

namespace Kysect.Shreks.Tests.Extensions;

public static class SubmissionExtensions
{
    public static Mentor GetMentor(this Submission submission)
    {
        return submission.GroupAssignment.GetMentor();
    }
}