using Kysect.Shreks.Core.Users;
using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Github.Queries;

public static class GetMentorByUsername
{
    public record Query(string Username) : IRequest<Response>;

    public record Response(Mentor Mentor);
}