using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.WebApi.Abstractions.Models.GroupAssignments;

namespace ITMO.Dev.ASAP.WebApi.Sdk.ControllerClients;

public interface IGroupAssignmentClient
{
    Task<GroupAssignmentDto> UpdateGroupAssignmentAsync(
        Guid assignmentId,
        Guid groupId,
        UpdateGroupAssignmentRequest request,
        CancellationToken cancellationToken = default);
}