using System.Collections.ObjectModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Identity.Entities;
using Kysect.Shreks.Identity.Tools;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using static Kysect.Shreks.Application.Abstractions.Identity.Queries.Login;

namespace Kysect.Shreks.Application.Handlers.Identity;

public class LoginHandler : IRequestHandler<Query, Response>
{
    private readonly UserManager<ShreksIdentityUser> _userManager;
    private readonly IdentityConfiguration _configuration;

    public LoginHandler(UserManager<ShreksIdentityUser> userManager, IdentityConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(request.Username);

        if (user is null)
            throw new UnauthorizedException("You are not authorized");

        if (!await _userManager.CheckPasswordAsync(user, request.Password))
            throw new UnauthorizedException("You are not authorized");

        IList<string> roles = await _userManager.GetRolesAsync(user);

        IEnumerable<Claim> claims = roles
            .Select(userRole => new Claim(ClaimTypes.Role, userRole))
            .Append(new Claim(ClaimTypes.Name, user.UserName))
            .Append(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

        var token = GetToken(claims);
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new Response
        (
            Token: tokenString,
            Expires: token.ValidTo,
            Roles: new ReadOnlyCollection<string>(roles)
        );
    }

    private JwtSecurityToken GetToken(IEnumerable<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.Secret));

        var token = new JwtSecurityToken
        (
            issuer: _configuration.Issuer,
            audience: _configuration.Audience,
            expires: DateTime.UtcNow.AddHours(_configuration.ExpiresHours),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return token;
    }
}