using Microsoft.AspNetCore.Identity;

namespace ITMO.Dev.ASAP.Identity.Entities;

public class AsapIdentityRole : IdentityRole<Guid>
{
    public const string AdminRoleName = "Admin";

    public AsapIdentityRole(string roleName)
        : base(roleName) { }

    protected AsapIdentityRole() { }
}