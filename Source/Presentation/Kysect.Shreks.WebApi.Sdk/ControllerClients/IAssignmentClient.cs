using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.WebApi.Abstractions.Models;

namespace Kysect.Shreks.WebApi.Sdk.ControllerClients;

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