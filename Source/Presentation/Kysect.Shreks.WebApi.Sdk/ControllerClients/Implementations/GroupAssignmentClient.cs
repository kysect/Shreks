using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.WebApi.Abstractions.Models.GroupAssignments;
using Kysect.Shreks.WebApi.Sdk.Tools;
using System.Net.Http.Json;

namespace Kysect.Shreks.WebApi.Sdk.ControllerClients.Implementations;

internal class GroupAssignmentClient : IGroupAssignmentClient
{
    private readonly ClientRequestHandler _handler;

    public GroupAssignmentClient(HttpClient client)
    {
        _handler = new ClientRequestHandler(client);
    }

    public async Task<GroupAssignmentDto> CreateGroupAssignmentAsync(
        CreateGroupAssignmentRequest request,
        CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Post, "api/GroupAssignment")
        {
            Content = JsonContent.Create(request),
        };

        return await _handler.SendAsync<GroupAssignmentDto>(message, cancellationToken);
    }

    public async Task<GroupAssignmentDto> UpdateGroupAssignmentAsync(
        Guid assignmentId,
        Guid groupId,
        UpdateGroupAssignmentRequest request,
        CancellationToken cancellationToken = default)
    {
        string uri = $"api/Assignments/{assignmentId}/groups/{groupId}";
        using var message = new HttpRequestMessage(HttpMethod.Put, uri) { Content = JsonContent.Create(request), };

        return await _handler.SendAsync<GroupAssignmentDto>(message, cancellationToken);
    }
}