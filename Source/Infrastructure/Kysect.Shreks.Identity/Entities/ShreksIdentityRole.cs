using Microsoft.AspNetCore.Identity;

namespace Kysect.Shreks.Identity.Entities;

public class ShreksIdentityRole : IdentityRole<Guid>
{
    public const string AdminRoleName = "Admin";

    public ShreksIdentityRole(string roleName) : base(roleName) { }

    protected ShreksIdentityRole() { }
}