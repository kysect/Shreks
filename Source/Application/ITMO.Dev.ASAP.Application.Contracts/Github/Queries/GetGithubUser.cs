using ITMO.Dev.ASAP.Application.Dto.Users;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Github.Queries;

internal static class GetGithubUser
{
    public record Query(string Username) : IRequest<Response>;

    public record Response(UserDto User);
}