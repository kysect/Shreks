using Kysect.Shreks.Application.Dto.Study;
using MediatR;

namespace Kysect.Shreks.Application.Contracts.Study.Queries;

public static class GetGroupAssignments
{
    public record Query(Guid AssignmentId) : IRequest<Response>;

    public record Response(IReadOnlyCollection<GroupAssignmentDto> GroupAssignments);
}