using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.WebApi.Abstractions.Models;

namespace ITMO.Dev.ASAP.WebApi.Sdk.ControllerClients;

public interface IAssignmentClient
{
    Task<AssignmentDto> CreateAssignmentAsync(
        CreateAssignmentRequest request,
        CancellationToken cancellationToken = default);

    Task<AssignmentDto> UpdateAssignmentPointsAsync(
        Guid id,
        double minPoints,
        double maxPoints,
        CancellationToken cancellationToken = default);

    Task<AssignmentDto> GetAssignmentAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<GroupAssignmentDto>> GetGroupAssignmentsAsync(
        Guid assignmentId,
        CancellationToken cancellationToken = default);
}