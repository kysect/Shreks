using ITMO.Dev.ASAP.Core.Study;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITMO.Dev.ASAP.DataAccess.Configurations;

public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
{
    public void Configure(EntityTypeBuilder<Subject> builder)
    {
        builder.Navigation(x => x.Courses).HasField("_courses");
    }
}