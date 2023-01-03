using Kysect.Shreks.WebApi.Abstractions.Models.Identity;
using Kysect.Shreks.WebApi.Sdk.Tools;
using System.Net.Http.Json;

namespace Kysect.Shreks.WebApi.Sdk.ControllerClients.Implementations;

internal class IdentityClient : IIdentityClient
{
    private readonly ClientRequestHandler _handler;

    public IdentityClient(HttpClient client)
    {
        _handler = new ClientRequestHandler(client);
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Post, "api/identity/login")
        {
            Content = JsonContent.Create(request),
        };

        return await _handler.SendAsync<LoginResponse>(message, cancellationToken);
    }

    public async Task PromoteAsync(string username, CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Post, $"api/identity/users/{username}/promote");

        await _handler.SendAsync(message, cancellationToken);
    }

    public async Task<LoginResponse> RegisterAsync(
        RegisterUserRequest request,
        CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Post, "api/identity/register")
        {
            Content = JsonContent.Create(request),
        };

        return await _handler.SendAsync<LoginResponse>(message, cancellationToken);
    }
}