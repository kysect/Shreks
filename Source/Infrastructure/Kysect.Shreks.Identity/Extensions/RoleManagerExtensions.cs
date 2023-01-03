using Kysect.Shreks.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace Kysect.Shreks.Identity.Extensions;

public static class RoleManagerExtensions
{
    public static async Task CreateRoleIfNotExistsAsync(
        this RoleManager<ShreksIdentityRole> roleManager,
        string roleName)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
            await roleManager.CreateAsync(new ShreksIdentityRole(roleName));
    }
}