using Kysect.Shreks.Application.Dto.Users;
using MediatR;

namespace Kysect.Shreks.Application.Contracts.Students.Commands;

internal static class CreateStudent
{
    public record Command(string FirstName, string MiddleName, string LastName, Guid GroupId) : IRequest<Response>;

    public record Response(StudentDto Student);
}