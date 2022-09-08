using Kysect.Shreks.Core.Queue.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kysect.Shreks.DataAccess.Configurations.Queue.Filters;

public class GroupQueueFilterConfiguration : IEntityTypeConfiguration<GroupQueueFilter>
{
    public void Configure(EntityTypeBuilder<GroupQueueFilter> builder)
    {
        builder.HasMany(x => x.Groups).WithMany("Filters");
    }
}