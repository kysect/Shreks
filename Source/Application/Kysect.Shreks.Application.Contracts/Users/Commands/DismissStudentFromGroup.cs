using MediatR;

namespace Kysect.Shreks.Application.Contracts.Users.Commands;

internal static class DismissStudentFromGroup
{
    public record Command(Guid StudentId) : IRequest;
}