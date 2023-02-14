using ITMO.Dev.ASAP.Application.Dto.Study;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Study.Subjects.Commands;

internal static class UpdateSubject
{
    public record Command(Guid Id, string NewName) : IRequest<Response>;

    public record Response(SubjectDto Subject);
}