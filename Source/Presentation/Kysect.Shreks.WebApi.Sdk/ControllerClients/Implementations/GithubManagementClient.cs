using Kysect.Shreks.WebApi.Sdk.Tools;

namespace Kysect.Shreks.WebApi.Sdk.ControllerClients.Implementations;

internal class GithubManagementClient : IGithubManagementClient
{
    private readonly ClientRequestHandler _handler;

    public GithubManagementClient(HttpClient client)
    {
        _handler = new ClientRequestHandler(client);
    }

    public async Task ForceOrganizationUpdateAsync(CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Post, "api/GithubManagement/force-update");
        await _handler.SendAsync(message, cancellationToken);
    }

    public async Task ForceMentorsSyncAsync(string organizationName, CancellationToken cancellationToken = default)
    {
        string uri = $"api/GithubManagement/force-sync?organizationName={organizationName}";
        using var message = new HttpRequestMessage(HttpMethod.Post, uri);

        await _handler.SendAsync(message, cancellationToken);
    }
}