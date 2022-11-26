using Kysect.Shreks.Application.Dto.Study;
using MediatR;

namespace Kysect.Shreks.Application.Contracts.Study.Queries;

internal static class GetSubjectById
{
    public record Query(Guid Id) : IRequest<Response>;

    public record Response(SubjectDto Subject);
}