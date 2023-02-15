using ITMO.Dev.ASAP.Core.Submissions;
using ITMO.Dev.ASAP.Core.Users;

namespace ITMO.Dev.ASAP.Tests.Extensions;

public static class SubmissionExtensions
{
    public static Mentor GetMentor(this Submission submission)
    {
        return submission.GroupAssignment.GetMentor();
    }
}