using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Kysect.Shreks.Application.Dto.Identity;
using Kysect.Shreks.Identity.Entities;
using Kysect.Shreks.Identity.Tools;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using static Kysect.Shreks.Application.Contracts.Identity.Queries.GetIdentityUserByToken;

namespace Kysect.Shreks.Application.Handlers.Identity;

internal class GetIdentityUserByTokenHandler : IRequestHandler<Query, Response>
{
    private readonly UserManager<ShreksIdentityUser> _userManager;
    private readonly IdentityConfiguration _configuration;
    private readonly IMapper _mapper;

    public GetIdentityUserByTokenHandler(
        IdentityConfiguration configuration,
        UserManager<ShreksIdentityUser> userManager,
        IMapper mapper)
    {
        _configuration = configuration;
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration.Secret);
        var parameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };

        tokenHandler.ValidateToken(request.Token, parameters, out var validatedToken);

        var jwtToken = (JwtSecurityToken)validatedToken;
        var username = jwtToken.Claims.Single(x => x.Type.Equals(ClaimTypes.Name)).Value;

        var user = await _userManager.FindByNameAsync(username);
        var userDto = _mapper.Map<IdentityUserDto>(user);

        return new Response(userDto);
    }
}