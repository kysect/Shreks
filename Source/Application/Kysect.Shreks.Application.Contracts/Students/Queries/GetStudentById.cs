using Kysect.Shreks.Application.Dto.Users;
using MediatR;

namespace Kysect.Shreks.Application.Contracts.Students.Queries;

public static class GetStudentById
{
    public record Query(Guid UserId) : IRequest<Response>;

    public record Response(StudentDto Student);
}