namespace ITMO.Dev.ASAP.WebApi.Sdk.ControllerClients;

public interface IGithubManagementClient
{
    Task ForceOrganizationUpdateAsync(CancellationToken cancellationToken = default);

    Task ForceMentorsSyncAsync(string organizationName, CancellationToken cancellationToken = default);
}