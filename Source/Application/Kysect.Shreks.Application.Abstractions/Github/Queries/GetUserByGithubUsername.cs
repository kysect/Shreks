using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Github.Queries;

public static class GetUserByGithubUsername
{
    public record Query(string Username) : IRequest<Response>;

    public record Response(Guid UserId);
}