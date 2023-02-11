using ITMO.Dev.ASAP.Application.Dto.Users;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Students.Queries;

internal static class GetStudentById
{
    public record Query(Guid UserId) : IRequest<Response>;

    public record Response(StudentDto Student);
}