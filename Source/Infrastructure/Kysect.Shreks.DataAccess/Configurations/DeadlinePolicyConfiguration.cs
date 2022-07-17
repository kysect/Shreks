using Kysect.Shreks.Core.DeadlinePolicies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kysect.Shreks.DataAccess.Configurations;

public class DeadlinePolicyConfiguration : IEntityTypeConfiguration<DeadlinePolicy>
{
    public void Configure(EntityTypeBuilder<DeadlinePolicy> builder)
    {
        builder.Property<Guid>("Id");
        builder.HasKey("Id");

        builder.HasDiscriminator<string>("DeadlinePolicyType")
            .HasValue<AbsoluteDeadlinePolicy>(nameof(AbsoluteDeadlinePolicy))
            .HasValue<FractionDeadlinePolicy>(nameof(FractionDeadlinePolicy))
            .HasValue<CappingDeadlinePolicy>(nameof(CappingDeadlinePolicy));
    }
}