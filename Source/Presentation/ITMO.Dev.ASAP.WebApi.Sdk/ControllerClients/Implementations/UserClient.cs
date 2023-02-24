using ITMO.Dev.ASAP.Application.Dto.Users;
using ITMO.Dev.ASAP.WebApi.Sdk.Tools;
using Newtonsoft.Json;

namespace ITMO.Dev.ASAP.WebApi.Sdk.ControllerClients.Implementations;

internal class UserClient : IUserClient
{
    private readonly ClientRequestHandler _handler;

    public UserClient(HttpClient client, JsonSerializerSettings serializerSettings)
    {
        _handler = new ClientRequestHandler(client, serializerSettings);
    }

    public async Task UpdateUniversityIdAsync(
        Guid userId,
        int universityId,
        CancellationToken cancellationToken = default)
    {
        string uri = $"api/User/{userId}/universityId/update?universityId={universityId}";
        using var message = new HttpRequestMessage(HttpMethod.Post, uri);

        await _handler.SendAsync(message, cancellationToken);
    }

    public async Task<UserDto?> FindUserByUniversityIdAsync(
        int universityId,
        CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Get, $"api/User?universityId={universityId}");
        return await _handler.SendAsync<UserDto?>(message, cancellationToken);
    }

    public async Task UpdateNameAsync(
        Guid userId,
        string firstName,
        string middleName,
        string lastName,
        CancellationToken cancellationToken = default)
    {
        string uri = $"api/User/{userId}/change-name?firstName={firstName}&middleName={middleName}&lastName={lastName}";
        using var message = new HttpRequestMessage(HttpMethod.Post, uri);

        await _handler.SendAsync(message, cancellationToken);
    }
}