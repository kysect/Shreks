using Kysect.Shreks.Application.Dto.Users;
using MediatR;

namespace Kysect.Shreks.Application.Contracts.Students.Commands;

public static class TransferStudent
{
    public record Command(Guid StudentId, Guid GroupId) : IRequest<Response>;

    public record Response(StudentDto Student);
}