using Kysect.Shreks.Core.Queue;
using Kysect.Shreks.Core.Queue.Evaluators;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kysect.Shreks.DataAccess.Configurations;

public class SubmissionEvaluatorsConfiguration : IEntityTypeConfiguration<SubmissionEvaluator>
{
    public void Configure(EntityTypeBuilder<SubmissionEvaluator> builder)
    {
        builder.Property<Guid>("SubmissionQueueId");

        builder.HasDiscriminator<string>("Discriminator")
            .HasValue<DeadlineEvaluator>(nameof(DeadlineEvaluator));

        builder.HasOne<SubmissionQueue>("SubmissionQueue")
            .WithMany(x => x.Evaluators);
    }
}