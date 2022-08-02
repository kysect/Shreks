using Kysect.Shreks.Core.Users;
using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Github.Queries;

public static class GetUserByUsername
{
    public record Query(string Username) : IRequest<Response>;

    public record Response(User UserId);
}