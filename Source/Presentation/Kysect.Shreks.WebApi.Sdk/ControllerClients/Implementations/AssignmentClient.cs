using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.WebApi.Abstractions.Models;
using Kysect.Shreks.WebApi.Sdk.Tools;
using System.Net.Http.Json;

namespace Kysect.Shreks.WebApi.Sdk.ControllerClients.Implementations;

internal class AssignmentClient : IAssignmentClient
{
    private readonly ClientRequestHandler _handler;

    public AssignmentClient(HttpClient client)
    {
        _handler = new ClientRequestHandler(client);
    }

    public async Task<AssignmentDto> CreateAssignmentAsync(
        CreateAssignmentRequest request,
        CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(HttpMethod.Post, "api/Assignments/")
        {
            Content = JsonContent.Create(request),
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