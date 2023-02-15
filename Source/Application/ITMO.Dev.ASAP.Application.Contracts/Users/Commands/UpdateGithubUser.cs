using ITMO.Dev.ASAP.Application.Dto.Users;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Users.Commands;

internal static class UpdateGithubUser
{
    public record Command(Guid UserId, string GithubUsername) : IRequest<Response>;

    public record Response(UserDto User);
}