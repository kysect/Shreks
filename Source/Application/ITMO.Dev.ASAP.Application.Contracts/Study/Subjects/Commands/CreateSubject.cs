using ITMO.Dev.ASAP.Application.Dto.Study;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Study.Subjects.Commands;

internal static class CreateSubject
{
    public record Command(string Title) : IRequest<Response>;

    public record Response(SubjectDto Subject);
}