using Kysect.Shreks.Application.Dto.Study;
using MediatR;

namespace Kysect.Shreks.Application.Contracts.Study.Queries;

internal static class FindStudyGroupByName
{
    public record Query(string Name) : IRequest<Response>;

    public record Response(StudyGroupDto? Group);
}