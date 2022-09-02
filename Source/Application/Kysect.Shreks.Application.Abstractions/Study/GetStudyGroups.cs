using Kysect.Shreks.Application.Dto.Study;
using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Study;

public static class GetStudyGroups
{
    public record Query() : IRequest<Response>;

    public record Response(IReadOnlyCollection<StudyGroupDto> Groups);
}