using ITMO.Dev.ASAP.Identity.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ITMO.Dev.ASAP.Identity;

public sealed class AsapIdentityContext : IdentityDbContext<AsapIdentityUser, AsapIdentityRole, Guid>
{
    public AsapIdentityContext(DbContextOptions<AsapIdentityContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }
}