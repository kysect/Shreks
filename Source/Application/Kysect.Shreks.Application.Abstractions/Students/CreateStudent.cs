using Kysect.Shreks.Application.Dto.Users;
using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Students;

public static class CreateStudent
{
    public record Command(string FirstName, string MiddleName, string LastName, Guid GroupId, int IsuId) : IRequest<Response>;

    public record Response(StudentDto Student);
}