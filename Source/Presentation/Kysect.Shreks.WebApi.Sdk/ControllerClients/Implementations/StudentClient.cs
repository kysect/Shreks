using Kysect.Shreks.Application.Dto.Querying;
using Kysect.Shreks.Application.Dto.Users;
using Kysect.Shreks.WebApi.Abstractions.Models.Students;
using Kysect.Shreks.WebApi.Sdk.Extensions;
using Kysect.Shreks.WebApi.Sdk.Tools;
using Newtonsoft.Json;

namespace Kysect.Shreks.WebApi.Sdk.ControllerClients.Implementations;

internal class StudentClient : IStudentClient
{
    private readonly ClientRequestHandler _handler;
    private readonly JsonSerializerSettings _serializerSettings;

    public StudentClient(HttpClient client, JsonSerializerSettings serializerSettings)
    {
        _serializerSettings = serializerSettings;
        _handler = new ClientRequestHandler(client, serializerSettings);
    }

    public async Task<StudentDto> CreateAsync(
        CreateStudentRequest request,
        CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Post, "api/Student")
        {
            Content = request.ToContent(_serializerSettings),
        };

        return await _handler.SendAsync<StudentDto>(message, cancellationToken);
    }

    public async Task<StudentDto> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Get, $"api/Student/{id}");
        return await _handler.SendAsync<StudentDto>(message, cancellationToken);
    }

    public async Task DismissFromGroupAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Put, $"api/Student/{id}/dismiss");
        await _handler.SendAsync(message, cancellationToken);
    }

    public async Task<StudentDto> TransferStudentAsync(
        Guid id,
        TransferStudentRequest request,
        CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Put, $"api/Student/{id}/group")
        {
            Content = request.ToContent(_serializerSettings),
        };

        return await _handler.SendAsync<StudentDto>(message, cancellationToken);
    }

    public async Task AddGithubAssociationAsync(
        Guid id,
        string githubUsername,
        CancellationToken cancellationToken = default)
    {
        string uri = $"api/Student/{id}/association/github?githubUsername={githubUsername}";
        using var message = new HttpRequestMessage(HttpMethod.Post, uri);

        await _handler.SendAsync(message, cancellationToken);
    }

    public async Task RemoveGithubAssociationAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Delete, $"api/Student/{id}/association/github");
        await _handler.SendAsync(message, cancellationToken);
    }

    public async Task<IReadOnlyCollection<StudentDto>> QueryAsync(
        QueryConfiguration<StudentQueryParameter> configuration,
        CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Post, "api/Student/query")
        {
            Content = configuration.ToContent(_serializerSettings),
        };

        return await _handler.SendAsync<IReadOnlyCollection<StudentDto>>(message, cancellationToken);
    }
}