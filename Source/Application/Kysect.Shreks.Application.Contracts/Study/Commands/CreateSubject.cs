using Kysect.Shreks.Application.Dto.Study;
using MediatR;

namespace Kysect.Shreks.Application.Contracts.Study.Commands;

public static class CreateSubject
{
    public record Command(string Title) : IRequest<Response>;

    public record Response(SubjectDto Subject);
}