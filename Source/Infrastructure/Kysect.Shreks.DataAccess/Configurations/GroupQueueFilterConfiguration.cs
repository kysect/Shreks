using Kysect.Shreks.Core.Queue.Filters;
using Kysect.Shreks.Core.Study;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kysect.Shreks.DataAccess.Configurations;

public class GroupQueueFilterConfiguration : IEntityTypeConfiguration<GroupQueueFilter>
{
    public void Configure(EntityTypeBuilder<GroupQueueFilter> builder)
    {
        builder
            .HasMany(x => x.Groups)
            .WithMany("Filters")
            .UsingEntity
            (
                "GroupFilterGroups",
                x => x.HasOne(typeof(StudentGroup)).WithMany().HasForeignKey("GroupId"),
                x => x.HasOne(typeof(GroupQueueFilter)).WithMany().HasForeignKey("FilterId")
            );
    }
}