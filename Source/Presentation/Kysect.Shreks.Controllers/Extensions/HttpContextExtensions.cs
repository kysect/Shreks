using Kysect.Shreks.Application.Dto.Identity;
using Kysect.Shreks.Common.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Kysect.Shreks.Controllers.Extensions;

public static class HttpContextExtensions
{
    public static IdentityUserDto GetUser(this HttpContext context)
    {
        if (context.Items["user"] is not IdentityUserDto user)
            throw new UnauthorizedException("User is not authorized");

        return user;
    }
}