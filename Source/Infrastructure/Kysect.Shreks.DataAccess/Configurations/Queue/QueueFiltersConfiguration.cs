using Kysect.Shreks.Core.Queue.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kysect.Shreks.DataAccess.Configurations.Queue;

public class QueueFiltersConfiguration : IEntityTypeConfiguration<QueueFilter>
{
    public void Configure(EntityTypeBuilder<QueueFilter> builder)
    {
        builder.HasDiscriminator<string>("Discriminator")
            .HasValue<GroupQueueFilter>(nameof(GroupQueueFilter))
            .HasValue<AssignmentGroupFilter>(nameof(AssignmentGroupFilter));
    }
}