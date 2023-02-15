using ITMO.Dev.ASAP.Application.Dto.Study;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Study.GroupAssignments.Queries;

internal static class GetGroupAssignments
{
    public record Query(Guid AssignmentId) : IRequest<Response>;

    public record Response(IReadOnlyCollection<GroupAssignmentDto> GroupAssignments);
}