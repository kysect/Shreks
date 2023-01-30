using Kysect.Shreks.Application.Dto.Querying;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Dto.Users;
using Kysect.Shreks.WebApi.Abstractions.Models.StudyGroups;
using Kysect.Shreks.WebApi.Sdk.Extensions;
using Kysect.Shreks.WebApi.Sdk.Tools;
using Newtonsoft.Json;

namespace Kysect.Shreks.WebApi.Sdk.ControllerClients.Implementations;

internal class StudyGroupClient : IStudyGroupClient
{
    private readonly ClientRequestHandler _handler;
    private readonly JsonSerializerSettings _serializerSettings;

    public StudyGroupClient(HttpClient client, JsonSerializerSettings serializerSettings)
    {
        _serializerSettings = serializerSettings;
        _handler = new ClientRequestHandler(client, serializerSettings);
    }

    public async Task<StudyGroupDto> CreateAsync(
        CreateStudyGroupRequest request,
        CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Post, "api/StudyGroup")
        {
            Content = request.ToContent(_serializerSettings),
        };

        return await _handler.SendAsync<StudyGroupDto>(message, cancellationToken);
    }

    public async Task<StudyGroupDto> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Get, $"api/StudyGroup/{id}");
        return await _handler.SendAsync<StudyGroupDto>(message, cancellationToken);
    }

    public async Task<IReadOnlyCollection<StudyGroupDto>> GetAsync(CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Get, "api/StudyGroup");
        return await _handler.SendAsync<IReadOnlyCollection<StudyGroupDto>>(message, cancellationToken);
    }

    public async Task<IReadOnlyCollection<StudyGroupDto>> GetAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default)
    {
        string uri = $"api/StudyGroup?ids={string.Join(",", ids)}";
        using var message = new HttpRequestMessage(HttpMethod.Get, uri);

        return await _handler.SendAsync<IReadOnlyCollection<StudyGroupDto>>(message, cancellationToken);
    }

    public async Task<IReadOnlyCollection<StudentDto>> GetStudentsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Get, $"api/StudyGroup/{id}/students");
        return await _handler.SendAsync<IReadOnlyCollection<StudentDto>>(message, cancellationToken);
    }

    public async Task<IReadOnlyCollection<GroupAssignmentDto>> GetAssignmentsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Get, $"api/StudyGroup/{id}/assignments");
        return await _handler.SendAsync<IReadOnlyCollection<GroupAssignmentDto>>(message, cancellationToken);
    }

    public async Task<StudyGroupDto?> FindAsync(string name, CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Get, $"api/StudyGroup/find?name={name}");
        return await _handler.SendAsync<StudyGroupDto?>(message, cancellationToken);
    }

    public async Task<StudyGroupDto> UpdateAsync(
        Guid id,
        UpdateStudyGroupRequest request,
        CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Put, $"api/StudyGroup/{id}")
        {
            Content = request.ToContent(_serializerSettings),
        };

        return await _handler.SendAsync<StudyGroupDto>(message, cancellationToken);
    }

    public async Task<IReadOnlyCollection<StudyGroupDto>> QueryAsync(
        QueryConfiguration<GroupQueryParameter> configuration,
        CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Post, "api/StudyGroup/query")
        {
            Content = configuration.ToContent(_serializerSettings),
        };

        return await _handler.SendAsync<IReadOnlyCollection<StudyGroupDto>>(message, cancellationToken);
    }
}