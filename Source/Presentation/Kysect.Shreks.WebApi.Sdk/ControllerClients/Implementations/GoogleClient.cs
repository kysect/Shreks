using Kysect.Shreks.WebApi.Sdk.Tools;

namespace Kysect.Shreks.WebApi.Sdk.ControllerClients.Implementations;

internal class GoogleClient : IGoogleClient
{
    private readonly ClientRequestHandler _handler;

    public GoogleClient(HttpClient client)
    {
        _handler = new ClientRequestHandler(client);
    }

    public async Task ForceSubjectCourseTableSyncAsync(
        Guid subjectCourseId,
        CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Post, "api/Google/force-sync");

        await _handler.SendAsync(message, cancellationToken);
    }
}