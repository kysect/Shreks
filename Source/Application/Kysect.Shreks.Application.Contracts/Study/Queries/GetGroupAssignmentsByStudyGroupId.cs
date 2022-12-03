using Kysect.Shreks.Application.Dto.Study;
using MediatR;

namespace Kysect.Shreks.Application.Contracts.Study.Queries;

internal static class GetGroupAssignmentsByStudyGroupId
{
    public record Query(Guid GroupId) : IRequest<Response>;

    public record Response(IReadOnlyCollection<GroupAssignmentDto> GroupAssignments);
}