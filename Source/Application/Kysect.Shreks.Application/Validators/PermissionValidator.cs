using Kysect.Shreks.Application.Abstractions.Permissions;
using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.Extensions;
using Kysect.Shreks.Core.Specifications.Github;
using Kysect.Shreks.DataAccess.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Kysect.Shreks.Application.Validators;

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

    public async Task EnsureSubmissionMentorAsync(
        Guid userId,
        Guid submissionId,
        CancellationToken cancellationToken)
    {
        bool isMentor = await _context.Submissions
            .Where(x => x.Id.Equals(submissionId))
            .Select(x => x.GroupAssignment.Assignment.SubjectCourse)
            .SelectMany(x => x.Mentors)
            .AnyAsync(x => x.UserId.Equals(userId), cancellationToken);

        if (isMentor is false)
            throw new UnauthorizedException();
    }

    public static bool IsRepositoryOwner(string username, string repositoryName)
    {
        return string.Equals(username, repositoryName, StringComparison.CurrentCultureIgnoreCase);
    }
}