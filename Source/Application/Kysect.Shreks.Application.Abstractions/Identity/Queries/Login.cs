using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Identity.Queries;

public static class Login
{
    public record Query(string Username, string Password) : IRequest<Response>;

    public record Response(string Token, DateTime Expires, IReadOnlyCollection<string> Roles);
}