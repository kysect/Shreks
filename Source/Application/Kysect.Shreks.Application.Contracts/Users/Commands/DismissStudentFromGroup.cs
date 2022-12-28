using MediatR;

namespace Kysect.Shreks.Application.Contracts.Users.Commands;

public static class DismissStudentFromGroup
{
    public record Command(Guid StudentId) : IRequest;
}