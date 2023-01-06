using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Identity.Entities;
using Kysect.Shreks.Identity.Tools;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static Kysect.Shreks.Application.Contracts.Identity.Queries.Login;

namespace Kysect.Shreks.Application.Handlers.Identity;

internal class LoginHandler : IRequestHandler<Query, Response>
{
    private readonly IdentityConfiguration _configuration;
    private readonly UserManager<ShreksIdentityUser> _userManager;

    public LoginHandler(UserManager<ShreksIdentityUser> userManager, IdentityConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        ShreksIdentityUser? user = await _userManager.FindByNameAsync(request.Username);

        if (user is null)
            throw new UnauthorizedException("You are not authorized");

        if (!await _userManager.CheckPasswordAsync(user, request.Password))
            throw new UnauthorizedException("You are not authorized");

        IList<string> roles = await _userManager.GetRolesAsync(user);

        IEnumerable<Claim> claims = roles
            .Select(userRole => new Claim(ClaimTypes.Role, userRole))
            .Append(new Claim(ClaimTypes.Name, user.UserName))
            .Append(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

        JwtSecurityToken token = GetToken(claims);
        string? tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new Response(
            tokenString,
            token.ValidTo,
            new ReadOnlyCollection<string>(roles));
    }

    private JwtSecurityToken GetToken(IEnumerable<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.Secret));

        var token = new JwtSecurityToken(
            _configuration.Issuer,
            _configuration.Audience,
            expires: DateTime.UtcNow.AddHours(_configuration.ExpiresHours),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

        return token;
    }
}