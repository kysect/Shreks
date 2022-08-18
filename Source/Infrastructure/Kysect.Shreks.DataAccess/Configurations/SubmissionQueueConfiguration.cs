using Kysect.Shreks.Core.Queue;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kysect.Shreks.DataAccess.Configurations;

public class SubmissionQueueConfiguration : IEntityTypeConfiguration<SubmissionQueue>
{
    public void Configure(EntityTypeBuilder<SubmissionQueue> builder)
    {
        builder.Navigation(x => x.Evaluators).HasField("_evaluators");
        builder.Navigation(x => x.Filters).HasField("_filters");

        builder.HasMany(x => x.Submissions);
    }
}