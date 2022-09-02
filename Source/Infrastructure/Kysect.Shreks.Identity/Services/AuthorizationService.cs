using Kysect.Shreks.Application.Abstractions.Identity;
using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace Kysect.Shreks.Identity.Services;

public class AuthorizationService : IAuthorizationService
{
    private readonly UserManager<ShreksIdentityUser> _userManager;

    public AuthorizationService(UserManager<ShreksIdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task AuthorizeAdminAsync(string username, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(username);
        
        if (user is not null && await _userManager.IsInRoleAsync(user, ShreksIdentityRole.AdminRoleName))
            return;
        
        throw new UnauthorizedException("User is not admin");
    }
}