namespace ITMO.Dev.ASAP.Application.Abstractions.Permissions;

public interface IPermissionValidator
{
    Task<bool> IsOrganizationMentor(Guid senderId, string organizationName);

    Task<bool> IsSubmissionMentorAsync(Guid userId, Guid submissionId, CancellationToken cancellationToken);

    Task EnsureSubmissionMentorAsync(Guid userId, Guid submissionId, CancellationToken cancellationToken);
}