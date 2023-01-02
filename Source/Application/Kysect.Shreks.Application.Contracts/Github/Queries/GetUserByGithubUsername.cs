using Kysect.Shreks.Application.Dto.Users;
using MediatR;

namespace Kysect.Shreks.Application.Contracts.Github.Queries;

internal static class GetUserByGithubUsername
{
    public record Query(string Username) : IRequest<Response>;

    public record Response(UserDto User);
}