using Kysect.Shreks.Core.Queue.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kysect.Shreks.DataAccess.Configurations.Queue.Filters;

public class AssignmentGroupFilterConfiguration : IEntityTypeConfiguration<AssignmentGroupFilter>
{
    public void Configure(EntityTypeBuilder<AssignmentGroupFilter> builder)
    {
        builder.HasMany(x => x.Assignments).WithMany("Filters");
    }
}