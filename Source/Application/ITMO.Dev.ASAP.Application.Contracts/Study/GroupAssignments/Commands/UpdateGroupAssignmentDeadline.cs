using ITMO.Dev.ASAP.Application.Dto.Study;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Study.GroupAssignments.Commands;

internal static class UpdateGroupAssignmentDeadline
{
    public record Command(Guid GroupId, Guid AssignmentId, DateOnly NewDeadline) : IRequest<Response>;

    public record Response(GroupAssignmentDto GroupAssignment);
}