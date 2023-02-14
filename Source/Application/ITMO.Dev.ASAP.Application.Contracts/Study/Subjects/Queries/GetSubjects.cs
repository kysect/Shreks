using ITMO.Dev.ASAP.Application.Dto.Study;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Study.Subjects.Queries;

internal static class GetSubjects
{
    public record Query : IRequest<Response>;

    public record Response(IReadOnlyCollection<SubjectDto> Subjects);
}