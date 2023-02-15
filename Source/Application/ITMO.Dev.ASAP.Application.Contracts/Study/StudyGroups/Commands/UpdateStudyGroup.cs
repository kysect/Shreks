using ITMO.Dev.ASAP.Application.Dto.Study;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Study.StudyGroups.Commands;

internal static class UpdateStudyGroup
{
    public record Command(Guid Id, string NewName) : IRequest<Response>;

    public record Response(StudyGroupDto Group);
}