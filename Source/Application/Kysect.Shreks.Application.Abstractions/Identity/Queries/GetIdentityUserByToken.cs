using Kysect.Shreks.Application.Dto.Identity;
using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Identity.Queries;

public static class GetIdentityUserByToken
{
    public record Query(string Token) : IRequest<Response>;

    public record Response(IdentityUserDto User);
}