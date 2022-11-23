using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.Extensions;
using Kysect.Shreks.Core.Specifications.Github;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.DataAccess.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Kysect.Shreks.Application.Validators;

public interface IPermissionValidator
{
    Task<bool> IsOrganizationMentor(Guid senderId, string organizationName);
}

public static class PermissionValidatorExtensions
{
    public static async Task EnsureUserIsOrganizationMentor(this IPermissionValidator permissionValidator, Guid senderId, string organizationName)
    {
        if (!await permissionValidator.IsOrganizationMentor(senderId, organizationName))
            throw new UnauthorizedException();
    }
}

public class PermissionValidator : IPermissionValidator
{
    private readonly IShreksDatabaseContext _context;

    public PermissionValidator(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public async Task<bool> IsOrganizationMentor(Guid senderId, string organizationName)
    {
        return await _context.SubjectCourseAssociations
            .WithSpecification(new FindMentorByUsernameAndOrganization(senderId, organizationName))
            .AnyAsync();
    }

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
        return await new PermissionValidator(context).IsOrganizationMentor(userId, organizationName);
    }
}