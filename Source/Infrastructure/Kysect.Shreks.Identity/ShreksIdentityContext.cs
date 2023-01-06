using Kysect.Shreks.Identity.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Kysect.Shreks.Identity;

public sealed class ShreksIdentityContext : IdentityDbContext<ShreksIdentityUser, ShreksIdentityRole, Guid>
{
    public ShreksIdentityContext(DbContextOptions<ShreksIdentityContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }
}