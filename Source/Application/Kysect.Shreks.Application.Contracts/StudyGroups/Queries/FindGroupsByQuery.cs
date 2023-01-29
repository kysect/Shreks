using Kysect.Shreks.Application.Dto.Querying;
using Kysect.Shreks.Application.Dto.Study;
using MediatR;

namespace Kysect.Shreks.Application.Contracts.StudyGroups.Queries;

public static class FindGroupsByQuery
{
    public record Query(QueryConfiguration<GroupQueryParameter> Configuration) : IRequest<Response>;

    public record Response(IReadOnlyCollection<StudyGroupDto> Groups);
}