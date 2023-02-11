using ITMO.Dev.ASAP.Application.Dto.Querying;
using ITMO.Dev.ASAP.Application.Dto.Study;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Study.StudyGroups.Queries;

internal static class FindGroupsByQuery
{
    public record Query(QueryConfiguration<GroupQueryParameter> Configuration) : IRequest<Response>;

    public record Response(IReadOnlyCollection<StudyGroupDto> Groups);
}