using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Students;

public static class GetStudentByUsername
{
    public record Query(Guid UserId) : IRequest<Response>;

    public record Response(Guid StudentId);
}