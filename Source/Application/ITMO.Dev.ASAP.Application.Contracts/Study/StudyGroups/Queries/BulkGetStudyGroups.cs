using ITMO.Dev.ASAP.Application.Dto.Study;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Study.StudyGroups.Queries;

internal static class BulkGetStudyGroups
{
    public record Query(IReadOnlyCollection<Guid> Ids) : IRequest<Response>;

    public record Response(IReadOnlyCollection<StudyGroupDto> Groups);
}