using Kysect.Shreks.Core.Queue.Evaluators;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kysect.Shreks.DataAccess.Configurations.Queue;

public class SubmissionEvaluatorsConfiguration : IEntityTypeConfiguration<SubmissionEvaluator>
{
    public void Configure(EntityTypeBuilder<SubmissionEvaluator> builder)
    {
        builder.HasDiscriminator<string>("Discriminator")
            .HasValue<AssignmentDeadlineStateEvaluator>(nameof(AssignmentDeadlineStateEvaluator))
            .HasValue<SubmissionDateTimeEvaluator>(nameof(SubmissionDateTimeEvaluator));
    }
}