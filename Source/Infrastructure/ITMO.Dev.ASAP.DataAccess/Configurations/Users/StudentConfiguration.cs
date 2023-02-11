using ITMO.Dev.ASAP.Core.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITMO.Dev.ASAP.DataAccess.Configurations.Users;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.HasKey(x => x.UserId);
        builder.HasOne(x => x.User).WithOne().HasForeignKey<Student>(x => x.UserId);
        builder.HasOne(x => x.Group);
    }
}