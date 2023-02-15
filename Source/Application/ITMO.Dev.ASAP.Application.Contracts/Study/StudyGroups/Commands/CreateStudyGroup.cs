using ITMO.Dev.ASAP.Application.Dto.Study;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Study.StudyGroups.Commands;

internal static class CreateStudyGroup
{
    public record Command(string Name) : IRequest<Response>;

    public record Response(StudyGroupDto Group);
}