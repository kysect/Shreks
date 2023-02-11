using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.WebApi.Abstractions.Models.SubjectCourseGroups;
using ITMO.Dev.ASAP.WebApi.Sdk.Extensions;
using ITMO.Dev.ASAP.WebApi.Sdk.Tools;
using Newtonsoft.Json;

namespace ITMO.Dev.ASAP.WebApi.Sdk.ControllerClients.Implementations;

internal class SubjectCourseGroupClient : ISubjectCourseGroupClient
{
    private readonly ClientRequestHandler _handler;
    private readonly JsonSerializerSettings _serializerSettings;

    public SubjectCourseGroupClient(HttpClient client, JsonSerializerSettings serializerSettings)
    {
        _serializerSettings = serializerSettings;
        _handler = new ClientRequestHandler(client, serializerSettings);
    }

    public async Task<SubjectCourseGroupDto> CreateAsync(
        CreateSubjectCourseGroupRequest request,
        CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Post, "api/SubjectCourseGroup")
        {
            Content = request.ToContent(_serializerSettings),
        };

        return await _handler.SendAsync<SubjectCourseGroupDto>(message, cancellationToken);
    }

    public async Task<IReadOnlyCollection<SubjectCourseGroupDto>> BulkCreateAsync(
        BulkCreateSubjectCourseGroupsRequest request,
        CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Post, "api/SubjectCourseGroup/bulk")
        {
            Content = request.ToContent(_serializerSettings),
        };

        return await _handler.SendAsync<IReadOnlyCollection<SubjectCourseGroupDto>>(message, cancellationToken);
    }

    public async Task DeleteAsync(
        DeleteSubjectCourseGroupRequest request,
        CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Delete, "api/SubjectCourseGroup")
        {
            Content = request.ToContent(_serializerSettings),
        };

        await _handler.SendAsync(message, cancellationToken);
    }
}