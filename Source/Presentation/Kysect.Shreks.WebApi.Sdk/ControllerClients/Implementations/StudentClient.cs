using Kysect.Shreks.Application.Dto.Querying;
using Kysect.Shreks.Application.Dto.Users;
using Kysect.Shreks.WebApi.Abstractions.Models.Students;
using Kysect.Shreks.WebApi.Sdk.Tools;
using System.Net.Http.Json;

namespace Kysect.Shreks.WebApi.Sdk.ControllerClients.Implementations;

internal class StudentClient : IStudentClient
{
    private readonly ClientRequestHandler _handler;

    public StudentClient(HttpClient client)
    {
        _handler = new ClientRequestHandler(client);
    }

    public async Task<StudentDto> CreateAsync(
        CreateStudentRequest request,
        CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Post, $"api/Student")
        {
            Content = JsonContent.Create(request),
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
        QueryConfiguration<StudentQueryParameter> query,
        CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Post, $"api/Student/query")
        {
            Content = JsonContent.Create(query),
        };

        return await _handler.SendAsync<IReadOnlyCollection<StudentDto>>(message, cancellationToken);
    }
}