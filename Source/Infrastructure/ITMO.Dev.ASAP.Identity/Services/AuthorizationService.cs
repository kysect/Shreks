using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace ITMO.Dev.ASAP.Identity.Services;

public class AuthorizationService : IAuthorizationService
{
    private readonly UserManager<AsapIdentityUser> _userManager;

    public AuthorizationService(UserManager<AsapIdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task AuthorizeAdminAsync(string username, CancellationToken cancellationToken)
    {
        AsapIdentityUser? user = await _userManager.FindByNameAsync(username);

        if (user is not null && await _userManager.IsInRoleAsync(user, AsapIdentityRole.AdminRoleName))
            return;

        throw new UnauthorizedException("User is not admin");
    }
}