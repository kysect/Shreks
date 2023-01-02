using Kysect.Shreks.Application.Dto.Study;
using MediatR;

namespace Kysect.Shreks.Application.Contracts.Study.Queries;

internal static class BulkGetStudyGroups
{
    public record Query(IReadOnlyCollection<Guid> Ids) : IRequest<Response>;

    public record Response(IReadOnlyCollection<StudyGroupDto> Groups);
}