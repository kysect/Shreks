namespace Kysect.Shreks.Application.Abstractions.Permissions;

public interface IPermissionValidator
{
    Task<bool> IsOrganizationMentor(Guid senderId, string organizationName);

    Task EnsureSubmissionMentorAsync(Guid userId, Guid submissionId, CancellationToken cancellationToken);
}