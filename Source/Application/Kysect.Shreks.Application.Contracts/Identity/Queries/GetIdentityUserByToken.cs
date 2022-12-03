using Kysect.Shreks.Application.Dto.Identity;
using MediatR;

namespace Kysect.Shreks.Application.Contracts.Identity.Queries;

internal static class GetIdentityUserByToken
{
    public record Query(string Token) : IRequest<Response>;

    public record Response(IdentityUserDto User);
}