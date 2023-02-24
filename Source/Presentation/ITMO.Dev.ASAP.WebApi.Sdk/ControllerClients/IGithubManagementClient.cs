namespace ITMO.Dev.ASAP.WebApi.Sdk.ControllerClients;

public interface IGithubManagementClient
{
    Task ForceOrganizationsUpdateAsync(CancellationToken cancellationToken = default);

    Task ForceOrganizationUpdateAsync(Guid subjectCourseId, CancellationToken cancellationToken = default);

    Task ForceMentorsSyncAsync(string organizationName, CancellationToken cancellationToken = default);
}