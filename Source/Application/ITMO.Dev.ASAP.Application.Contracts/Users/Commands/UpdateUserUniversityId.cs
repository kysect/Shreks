using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Users.Commands;

internal static class UpdateUserUniversityId
{
    public record struct Command(string CallerUsername, Guid UserId, int UniversityId) : IRequest;
}