using ITMO.Dev.ASAP.Identity.Entities;
using ITMO.Dev.ASAP.Identity.Tools;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static ITMO.Dev.ASAP.Application.Contracts.Identity.Queries.GetIdentityUserByToken;

namespace ITMO.Dev.ASAP.Application.Handlers.Identity;

internal class GetIdentityUserByTokenHandler : IRequestHandler<Query, Response>
{
    private readonly IdentityConfiguration _configuration;
    private readonly UserManager<AsapIdentityUser> _userManager;

    public GetIdentityUserByTokenHandler(
        IdentityConfiguration configuration,
        UserManager<AsapIdentityUser> userManager)
    {
        _configuration = configuration;
        _userManager = userManager;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        byte[] key = Encoding.ASCII.GetBytes(_configuration.Secret);
        var parameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero,
        };

        tokenHandler.ValidateToken(request.Token, parameters, out SecurityToken? validatedToken);

        var jwtToken = (JwtSecurityToken)validatedToken;
        string username = jwtToken.Claims.Single(x => x.Type.Equals(ClaimTypes.Name, StringComparison.Ordinal)).Value;

        AsapIdentityUser? user = await _userManager.FindByNameAsync(username);

        return new Response(user.ToDto());
    }
}