using Kysect.Shreks.Core.Queue;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kysect.Shreks.DataAccess.Configurations;

public class PositionedSubmissionConfiguration : IEntityTypeConfiguration<PositionedSubmission>
{
    public void Configure(EntityTypeBuilder<PositionedSubmission> builder)
    {
        builder.HasOne(x => x.Submission);
    }
}