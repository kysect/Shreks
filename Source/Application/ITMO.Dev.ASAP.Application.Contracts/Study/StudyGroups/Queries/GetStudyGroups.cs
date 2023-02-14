using ITMO.Dev.ASAP.Application.Dto.Study;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Study.StudyGroups.Queries;

internal static class GetStudyGroups
{
    public record Query : IRequest<Response>;

    public record Response(IReadOnlyCollection<StudyGroupDto> Groups);
}