using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.WebApi.Abstractions.Models.GroupAssignments;

namespace Kysect.Shreks.WebApi.Sdk.ControllerClients;

public interface IGroupAssignmentClient
{
    Task<GroupAssignmentDto> CreateGroupAssignmentAsync(
        CreateGroupAssignmentRequest request,
        CancellationToken cancellationToken = default);

    Task<GroupAssignmentDto> UpdateGroupAssignmentAsync(
        Guid assignmentId,
        Guid groupId,
        UpdateGroupAssignmentRequest request,
        CancellationToken cancellationToken = default);
}