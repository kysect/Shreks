using Kysect.Shreks.Application.Dto.Users;
using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Students;

public static class GetStudentById
{
    public record Query(Guid UserId) : IRequest<Response>;

    public record Response(StudentDto Student);
}