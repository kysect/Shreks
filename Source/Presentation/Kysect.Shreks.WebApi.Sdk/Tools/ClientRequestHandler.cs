using Kysect.Shreks.WebApi.Sdk.Exceptions;
using System.Net;
using System.Net.Http.Json;

namespace Kysect.Shreks.WebApi.Sdk.Tools;

public class ClientRequestHandler
{
    private readonly HttpClient _client;

    public ClientRequestHandler(HttpClient client)
    {
        _client = client;
    }

    public async Task<T> SendAsync<T>(HttpRequestMessage message, CancellationToken cancellationToken)
    {
        HttpResponseMessage response = await _client.SendAsync(message, cancellationToken);

        if (response.StatusCode is HttpStatusCode.Unauthorized)
            throw new UnauthorizedException();

        if (response.IsSuccessStatusCode is false)
            throw RequestFailedException.Create("Failed to get sessions", response.StatusCode);

        T? content = await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);

        return content ?? throw RequestFailedException.Create("Failed to parse sessions", response.StatusCode);
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