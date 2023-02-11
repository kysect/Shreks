using ITMO.Dev.ASAP.Core.Study;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITMO.Dev.ASAP.DataAccess.Configurations;

public class SubjectCourseGroupConfiguration : IEntityTypeConfiguration<SubjectCourseGroup>
{
    public void Configure(EntityTypeBuilder<SubjectCourseGroup> builder)
    {
        builder.HasKey(x => new { x.SubjectCourseId, x.StudentGroupId });

        builder.HasOne(x => x.SubjectCourse);
        builder.HasOne(x => x.StudentGroup);
    }
}