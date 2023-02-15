using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Users.Commands;

internal static class DismissStudentFromGroup
{
    public record Command(Guid StudentId) : IRequest;
}