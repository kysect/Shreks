using ITMO.Dev.ASAP.Application.Dto.Study;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Study.StudyGroups.Queries;

internal static class GetStudyGroupById
{
    public record Query(Guid Id) : IRequest<Response>;

    public record Response(StudyGroupDto Group);
}