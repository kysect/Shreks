using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.Application.Dto.Users;
using ITMO.Dev.ASAP.WebApi.Abstractions.Models.SubjectCourses;
using ITMO.Dev.ASAP.WebApi.Sdk.Extensions;
using ITMO.Dev.ASAP.WebApi.Sdk.Tools;
using Newtonsoft.Json;

namespace ITMO.Dev.ASAP.WebApi.Sdk.ControllerClients.Implementations;

internal class SubjectCourseClient : ISubjectCourseClient
{
    private readonly ClientRequestHandler _handler;
    private readonly JsonSerializerSettings _serializerSettings;

    public SubjectCourseClient(HttpClient client, JsonSerializerSettings serializerSettings)
    {
        _serializerSettings = serializerSettings;
        _handler = new ClientRequestHandler(client, serializerSettings);
    }

    public async Task<SubjectCourseDto> CreateAsync(
        CreateSubjectCourseRequest request,
        CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Post, "api/SubjectCourse")
        {
            Content = request.ToContent(_serializerSettings),
        };

        return await _handler.SendAsync<SubjectCourseDto>(message, cancellationToken);
    }

    public async Task<IReadOnlyCollection<SubjectCourseDto>> GetAsync(CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Get, "api/SubjectCourse");
        return await _handler.SendAsync<IReadOnlyCollection<SubjectCourseDto>>(message, cancellationToken);
    }

    public async Task<SubjectCourseDto> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Get, $"api/SubjectCourse/{id}");
        return await _handler.SendAsync<SubjectCourseDto>(message, cancellationToken);
    }

    public async Task<SubjectCourseDto> UpdateAsync(
        Guid id,
        UpdateSubjectCourseRequest request,
        CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Put, $"api/SubjectCourse/{id}")
        {
            Content = request.ToContent(_serializerSettings),
        };

        return await _handler.SendAsync<SubjectCourseDto>(message, cancellationToken);
    }

    public async Task<IReadOnlyCollection<StudentDto>> GetStudentsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Get, $"api/SubjectCourse/{id}/students");
        return await _handler.SendAsync<IReadOnlyCollection<StudentDto>>(message, cancellationToken);
    }

    public async Task<IReadOnlyCollection<AssignmentDto>> GetAssignmentsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Get, $"api/SubjectCourse/{id}/assignments");
        return await _handler.SendAsync<IReadOnlyCollection<AssignmentDto>>(message, cancellationToken);
    }

    public async Task<IReadOnlyCollection<SubjectCourseGroupDto>> GetGroupsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Get, $"api/SubjectCourse/{id}/groups");
        return await _handler.SendAsync<IReadOnlyCollection<SubjectCourseGroupDto>>(message, cancellationToken);
    }

    public async Task<SubjectCourseDto> AddGithubAssociationAsync(
        Guid id,
        AddSubjectCourseGithubAssociationRequest request,
        CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Post, $"api/SubjectCourse/{id}/association/github")
        {
            Content = request.ToContent(_serializerSettings),
        };

        return await _handler.SendAsync<SubjectCourseDto>(message, cancellationToken);
    }

    public async Task<SubjectCourseDto> RemoveGithubAssociationAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Delete, $"api/SubjectCourse/{id}/association/github");
        return await _handler.SendAsync<SubjectCourseDto>(message, cancellationToken);
    }

    public async Task AddFractionDeadlinePolicyAsync(
        Guid id,
        AddFractionPolicyRequest request,
        CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Post, $"api/SubjectCourse/{id}/deadline/fraction")
        {
            Content = request.ToContent(_serializerSettings),
        };

        await _handler.SendAsync(message, cancellationToken);
    }
}