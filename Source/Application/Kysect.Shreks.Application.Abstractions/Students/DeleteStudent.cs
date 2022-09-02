using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Students;

public static class DeleteStudent
{
    public record Command(Guid Id) : IRequest<Response>;

    public record Response();
}