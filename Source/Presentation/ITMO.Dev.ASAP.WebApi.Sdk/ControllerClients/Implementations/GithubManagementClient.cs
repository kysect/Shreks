using ITMO.Dev.ASAP.WebApi.Sdk.Tools;
using Newtonsoft.Json;

namespace ITMO.Dev.ASAP.WebApi.Sdk.ControllerClients.Implementations;

internal class GithubManagementClient : IGithubManagementClient
{
    private readonly ClientRequestHandler _handler;

    public GithubManagementClient(HttpClient client, JsonSerializerSettings serializerSettings)
    {
        _handler = new ClientRequestHandler(client, serializerSettings);
    }

    public async Task ForceOrganizationsUpdateAsync(CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Post, "api/GithubManagement/force-update");
        await _handler.SendAsync(message, cancellationToken);
    }

    public async Task ForceOrganizationUpdateAsync(Guid subjectCourseId, CancellationToken cancellationToken = default)
    {
        string uri = $"api/GithubManagement/force-update?subjectCourseId={subjectCourseId}";
        using var message = new HttpRequestMessage(HttpMethod.Post, uri);

        await _handler.SendAsync(message, cancellationToken);
    }

    public async Task ForceMentorsSyncAsync(string organizationName, CancellationToken cancellationToken = default)
    {
        string uri = $"api/GithubManagement/force-mentor-sync?organizationName={organizationName}";
        using var message = new HttpRequestMessage(HttpMethod.Post, uri);

        await _handler.SendAsync(message, cancellationToken);
    }
}