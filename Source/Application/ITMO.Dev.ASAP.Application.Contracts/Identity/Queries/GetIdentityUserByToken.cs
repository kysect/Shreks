using ITMO.Dev.ASAP.Application.Dto.Identity;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Identity.Queries;

internal static class GetIdentityUserByToken
{
    public record Query(string Token) : IRequest<Response>;

    public record Response(IdentityUserDto User);
}