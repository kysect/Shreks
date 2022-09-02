using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Users.Commands;

public static class UpdateUserUniversityId
{
    public record struct Command(string CallerUsername, Guid UserId, int UniversityId) : IRequest;
}