using ITMO.Dev.ASAP.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace ITMO.Dev.ASAP.Identity.Extensions;

public static class RoleManagerExtensions
{
    public static async Task CreateRoleIfNotExistsAsync(
        this RoleManager<AsapIdentityRole> roleManager,
        string roleName)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
            await roleManager.CreateAsync(new AsapIdentityRole(roleName));
    }
}