using Kysect.Shreks.Application.Abstractions.Exceptions;
using Kysect.Shreks.Core.Submissions;

namespace Kysect.Shreks.Application.Handlers.Validators;

public static class PermissionValidator
{
    public static void EnsureHasAccessToRepository(Guid userId, Submission submission)
    {
        if (!IsRepositoryOwner(userId, submission)
            && !IsRepositoryMentor(userId, submission))
        {
            const string message = "User is not authorized to activate this submission";
            throw new UnauthorizedException(message);
        }
    }

    public static bool IsRepositoryOwner(Guid userId, Submission submission)
    {
        return submission.Student.User.Id.Equals(userId);
    }

    public static bool IsRepositoryMentor(Guid userId, Submission submission)
    {
        return submission
            .GroupAssignment
            .Assignment
            .SubjectCourse
            .Mentors.Any(x => x.Id.Equals(userId));
    }
}