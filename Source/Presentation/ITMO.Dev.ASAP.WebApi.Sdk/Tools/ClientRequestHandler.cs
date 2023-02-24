using ITMO.Dev.ASAP.WebApi.Sdk.Exceptions;
using Newtonsoft.Json;
using System.Net;

namespace ITMO.Dev.ASAP.WebApi.Sdk.Tools;

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

        if (response.StatusCode is HttpStatusCode.NoContent)
            return default!;

        if (response.IsSuccessStatusCode is false)
        {
            throw RequestFailedException.Create(
                "Failed to get response",
                response.StatusCode,
                await response.Content.ReadAsStringAsync(default));
        }

        string content = await response.Content.ReadAsStringAsync(cancellationToken);
        T? value = JsonConvert.DeserializeObject<T>(content, _serializerSettings);

        return value ?? throw RequestFailedException.Create(
            "Failed to parse response",
            response.StatusCode,
            await response.Content.ReadAsStringAsync(default));
    }

    public async Task SendAsync(HttpRequestMessage message, CancellationToken cancellationToken)
    {
        HttpResponseMessage response = await _client.SendAsync(message, cancellationToken);

        if (response.StatusCode is HttpStatusCode.Unauthorized)
            throw new UnauthorizedException();

        if (response.IsSuccessStatusCode is false)
        {
            throw RequestFailedException.Create(
                "Failed to get response",
                response.StatusCode,
                await response.Content.ReadAsStringAsync(default));
        }
    }
}