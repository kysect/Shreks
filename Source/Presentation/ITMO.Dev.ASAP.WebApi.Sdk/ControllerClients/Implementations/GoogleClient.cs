using ITMO.Dev.ASAP.WebApi.Sdk.Tools;
using Newtonsoft.Json;

namespace ITMO.Dev.ASAP.WebApi.Sdk.ControllerClients.Implementations;

internal class GoogleClient : IGoogleClient
{
    private readonly ClientRequestHandler _handler;

    public GoogleClient(HttpClient client, JsonSerializerSettings serializerSettings)
    {
        _handler = new ClientRequestHandler(client, serializerSettings);
    }

    public async Task ForceSubjectCourseTableSyncAsync(
        Guid subjectCourseId,
        CancellationToken cancellationToken = default)
    {
        string uri = $"api/Google/force-sync?subjectCourseId={subjectCourseId}";
        using var message = new HttpRequestMessage(HttpMethod.Post, uri);

        await _handler.SendAsync(message, cancellationToken);
    }
}