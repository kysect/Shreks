using Kysect.Shreks.Application.Dto.Users;
using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Students.Commands;

public static class UpdateUserGithubUsername
{
    public record Command(Guid UserId, string GithubUsername) : IRequest<Response>;
    public record Response(UserDto User);
}