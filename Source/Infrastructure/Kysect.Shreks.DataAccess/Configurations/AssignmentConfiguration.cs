using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.ValueConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kysect.Shreks.DataAccess.Configurations;

public class AssignmentConfiguration : IEntityTypeConfiguration<Assignment>
{
    public void Configure(EntityTypeBuilder<Assignment> builder)
    {
        builder.Property(x => x.MinPoints).HasConversion<PointsValueConverter>();
        builder.Property(x => x.MaxPoints).HasConversion<PointsValueConverter>();
        
        builder.Navigation(x => x.GroupAssignments).HasField("_groupAssignments");
        builder.Navigation(x => x.DeadlinePolicies).HasField("_deadlinePolicies");
    }
}