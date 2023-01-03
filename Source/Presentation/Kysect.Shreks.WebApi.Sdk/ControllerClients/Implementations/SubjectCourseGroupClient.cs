using Kysect.Shreks.Application.Dto.SubjectCourses;
using Kysect.Shreks.WebApi.Abstractions.Models.SubjectCourseGroups;
using Kysect.Shreks.WebApi.Sdk.Tools;
using System.Net.Http.Json;

namespace Kysect.Shreks.WebApi.Sdk.ControllerClients.Implementations;

internal class SubjectCourseGroupClient : ISubjectCourseGroupClient
{
    private readonly ClientRequestHandler _handler;

    public SubjectCourseGroupClient(HttpClient client)
    {
        _handler = new ClientRequestHandler(client);
    }

    public async Task<SubjectCourseGroupDto> CreateAsync(
        CreateSubjectCourseGroupRequest request,
        CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Post, "api/SubjectCourseGroup")
        {
            Content = JsonContent.Create(request),
        };

        return await _handler.SendAsync<SubjectCourseGroupDto>(message, cancellationToken);
    }

    public async Task DeleteAsync(
        DeleteSubjectCourseGroupRequest request,
        CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Delete, "api/SubjectCourseGroup")
        {
            Content = JsonContent.Create(request),
        };

        await _handler.SendAsync(message, cancellationToken);
    }
}