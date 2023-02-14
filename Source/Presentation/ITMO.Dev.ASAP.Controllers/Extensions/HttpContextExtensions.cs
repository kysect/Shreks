using ITMO.Dev.ASAP.Application.Dto.Identity;
using ITMO.Dev.ASAP.Common.Exceptions;
using Microsoft.AspNetCore.Http;

namespace ITMO.Dev.ASAP.Controllers.Extensions;

public static class HttpContextExtensions
{
    public static IdentityUserDto GetUser(this HttpContext context)
    {
        if (context.Items["user"] is not IdentityUserDto user)
            throw new UnauthorizedException("User is not authorized");

        return user;
    }
}