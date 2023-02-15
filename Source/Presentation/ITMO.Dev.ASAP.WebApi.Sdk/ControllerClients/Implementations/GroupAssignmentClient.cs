using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.WebApi.Abstractions.Models.GroupAssignments;
using ITMO.Dev.ASAP.WebApi.Sdk.Extensions;
using ITMO.Dev.ASAP.WebApi.Sdk.Tools;
using Newtonsoft.Json;

namespace ITMO.Dev.ASAP.WebApi.Sdk.ControllerClients.Implementations;

internal class GroupAssignmentClient : IGroupAssignmentClient
{
    private readonly ClientRequestHandler _handler;
    private readonly JsonSerializerSettings _serializerSettings;

    public GroupAssignmentClient(HttpClient client, JsonSerializerSettings serializerSettings)
    {
        _serializerSettings = serializerSettings;
        _handler = new ClientRequestHandler(client, serializerSettings);
    }

    public async Task<GroupAssignmentDto> UpdateGroupAssignmentAsync(
        Guid assignmentId,
        Guid groupId,
        UpdateGroupAssignmentRequest request,
        CancellationToken cancellationToken = default)
    {
        string uri = $"api/Assignments/{assignmentId}/groups/{groupId}";
        using var message = new HttpRequestMessage(HttpMethod.Put, uri)
        {
            Content = request.ToContent(_serializerSettings),
        };

        return await _handler.SendAsync<GroupAssignmentDto>(message, cancellationToken);
    }
}