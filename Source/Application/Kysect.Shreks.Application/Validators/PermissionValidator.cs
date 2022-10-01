using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.Extensions;
using Kysect.Shreks.Core.Specifications.Github;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Kysect.Shreks.Application.Validators;

public static class PermissionValidator
{
    public static void EnsureHasAccessToRepository(Guid userId, Submission submission)
    {
        if (!IsRepositoryOwner(userId, submission)
            && !IsRepositoryMentor(userId, submission))
        {
            throw UnauthorizedException.DoesNotHavePermissionForActivateSubmission();
        }
    }

    public static void EnsureMentorAccess(Guid userId, Submission submission)
    {
        if (!IsRepositoryMentor(userId, submission))
            throw UnauthorizedException.DoesNotHavePermissionForChangeSubmission();
    }

    public static bool IsRepositoryOwner(Guid userId, Submission submission)
    {
        return submission.Student.User.Id.Equals(userId);
    }

    public static bool IsRepositoryOwner(string username, string repositoryName)
    {
        return string.Equals(username, repositoryName, StringComparison.CurrentCultureIgnoreCase);
    }

    public static bool IsRepositoryMentor(Guid userId, Submission submission)
    {
        return submission
            .GroupAssignment
            .Assignment
            .SubjectCourse
            .Mentors.Any(x => x.UserId.Equals(userId));
    }

    public static async Task<bool> IsOrganizationMentor(IShreksDatabaseContext context, Guid userId, string organizationName)
    {
        Mentor? mentor = await context.SubjectCourseAssociations
            .WithSpecification(new FindMentorByUsernameAndOrganization(userId, organizationName))
            .FirstOrDefaultAsync();

        return mentor is not null;
    }
}