using ITMO.Dev.ASAP.Core.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITMO.Dev.ASAP.DataAccess.Configurations.Users;

public class MentorConfiguration : IEntityTypeConfiguration<Mentor>
{
    public void Configure(EntityTypeBuilder<Mentor> builder)
    {
        builder.HasKey(x => new { x.UserId, x.CourseId });
        builder.HasOne(x => x.User);
        builder.HasOne(x => x.Course);
    }
}