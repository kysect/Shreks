using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.ValueConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kysect.Shreks.DataAccess.Configurations;

public class SubmissionConfiguration : IEntityTypeConfiguration<Submission>
{
    public void Configure(EntityTypeBuilder<Submission> builder)
    {
        builder.HasOne(x => x.Student);
        builder.HasOne(x => x.Assignment);
        builder.Navigation(x => x.Associations).HasField("_associations");
        builder.Property(x => x.Rating).HasConversion<FractionValueConverter>();
    }
}