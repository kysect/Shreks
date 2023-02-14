using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.DataAccess.ValueConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITMO.Dev.ASAP.DataAccess.Configurations;

public class AssignmentConfiguration : IEntityTypeConfiguration<Assignment>
{
    public void Configure(EntityTypeBuilder<Assignment> builder)
    {
        builder.Property(x => x.MinPoints).HasConversion<PointsValueConverter>();
        builder.Property(x => x.MaxPoints).HasConversion<PointsValueConverter>();

        builder.Navigation(x => x.GroupAssignments).HasField("_groupAssignments");

        builder.HasIndex("ShortName", "SubjectCourseId").IsUnique();
    }
}