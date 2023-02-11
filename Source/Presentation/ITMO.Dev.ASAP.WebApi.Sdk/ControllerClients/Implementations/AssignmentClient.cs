using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.WebApi.Abstractions.Models;
using ITMO.Dev.ASAP.WebApi.Sdk.Extensions;
using ITMO.Dev.ASAP.WebApi.Sdk.Tools;
using Newtonsoft.Json;

namespace ITMO.Dev.ASAP.WebApi.Sdk.ControllerClients.Implementations;

internal class AssignmentClient : IAssignmentClient
{
    private readonly ClientRequestHandler _handler;
    private readonly JsonSerializerSettings _serializerSettings;

    public AssignmentClient(HttpClient client, JsonSerializerSettings serializerSettings)
    {
        _serializerSettings = serializerSettings;
        _handler = new ClientRequestHandler(client, serializerSettings);
    }

    public async Task<AssignmentDto> CreateAssignmentAsync(
        CreateAssignmentRequest request,
        CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Post, "api/Assignments/")
        {
            Content = request.ToContent(_serializerSettings),
        };

        return await _handler.SendAsync<AssignmentDto>(message, cancellationToken);
    }

    public async Task<AssignmentDto> UpdateAssignmentPointsAsync(
        Guid id,
        double minPoints,
        double maxPoints,
        CancellationToken cancellationToken = default)
    {
        string uri = $"api/Assignments/{id}?minPoints={minPoints}&maxPoints={maxPoints}";
        using var message = new HttpRequestMessage(HttpMethod.Patch, uri);

        return await _handler.SendAsync<AssignmentDto>(message, cancellationToken);
    }

    public async Task<AssignmentDto> GetAssignmentAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Get, $"api/Assignments/{id}");
        return await _handler.SendAsync<AssignmentDto>(message, cancellationToken);
    }

    public async Task<IReadOnlyCollection<GroupAssignmentDto>> GetGroupAssignmentsAsync(
        Guid assignmentId,
        CancellationToken cancellationToken = default)
    {
        string uri = $"api/Assignments/{assignmentId}/groups";
        using var message = new HttpRequestMessage(HttpMethod.Get, uri);

        return await _handler.SendAsync<IReadOnlyCollection<GroupAssignmentDto>>(message, cancellationToken);
    }
}