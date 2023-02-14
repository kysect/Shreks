using ITMO.Dev.ASAP.Application.Dto.Study;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Study.StudyGroups.Queries;

internal static class FindStudyGroupByName
{
    public record Query(string Name) : IRequest<Response>;

    public record Response(StudyGroupDto? Group);
}