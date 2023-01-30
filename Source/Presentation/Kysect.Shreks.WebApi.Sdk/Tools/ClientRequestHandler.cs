using Kysect.Shreks.WebApi.Sdk.Exceptions;
using Newtonsoft.Json;
using System.Net;

namespace Kysect.Shreks.WebApi.Sdk.Tools;

public class ClientRequestHandler
{
    private readonly HttpClient _client;
    private readonly JsonSerializerSettings _serializerSettings;

    public ClientRequestHandler(HttpClient client, JsonSerializerSettings serializerSettings)
    {
        _client = client;
        _serializerSettings = serializerSettings;
    }

    public async Task<T> SendAsync<T>(HttpRequestMessage message, CancellationToken cancellationToken)
    {
        HttpResponseMessage response = await _client.SendAsync(message, cancellationToken);

        if (response.StatusCode is HttpStatusCode.Unauthorized)
            throw new UnauthorizedException();

        if (response.IsSuccessStatusCode is false)
            throw RequestFailedException.Create("Failed to get sessions", response.StatusCode);

        string content = await response.Content.ReadAsStringAsync(cancellationToken);
        T? value = JsonConvert.DeserializeObject<T>(content, _serializerSettings);

        return value ?? throw RequestFailedException.Create("Failed to parse sessions", response.StatusCode);
    }

    public async Task SendAsync(HttpRequestMessage message, CancellationToken cancellationToken)
    {
        HttpResponseMessage response = await _client.SendAsync(message, cancellationToken);

        if (response.StatusCode is HttpStatusCode.Unauthorized)
            throw new UnauthorizedException();

        if (response.IsSuccessStatusCode is false)
            throw RequestFailedException.Create("Failed to get sessions", response.StatusCode);
    }
}