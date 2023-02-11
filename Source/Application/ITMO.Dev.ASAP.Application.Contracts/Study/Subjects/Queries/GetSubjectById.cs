using ITMO.Dev.ASAP.Application.Dto.Study;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Study.Subjects.Queries;

internal static class GetSubjectById
{
    public record Query(Guid Id) : IRequest<Response>;

    public record Response(SubjectDto Subject);
}