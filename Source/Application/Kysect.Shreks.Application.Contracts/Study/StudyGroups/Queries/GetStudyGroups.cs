using Kysect.Shreks.Application.Dto.Study;
using MediatR;

namespace Kysect.Shreks.Application.Contracts.Study.StudyGroups.Queries;

internal static class GetStudyGroups
{
    public record Query : IRequest<Response>;

    public record Response(IReadOnlyCollection<StudyGroupDto> Groups);
}