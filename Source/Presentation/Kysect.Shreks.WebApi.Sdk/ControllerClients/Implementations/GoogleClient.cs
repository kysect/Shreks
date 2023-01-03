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
        string uri = $"api/Google/force-sync?subjectCourseId={subjectCourseId}";
        using var message = new HttpRequestMessage(HttpMethod.Post, uri);

        await _handler.SendAsync(message, cancellationToken);
    }
}