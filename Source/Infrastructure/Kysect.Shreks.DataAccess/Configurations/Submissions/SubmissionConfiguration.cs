using Kysect.Shreks.Core.Submissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kysect.Shreks.DataAccess.Configurations.Submissions;

public class SubmissionConfiguration : IEntityTypeConfiguration<Submission>
{
    public void Configure(EntityTypeBuilder<Submission> builder)
    {
        builder.HasOne(x => x.Student);
        builder.HasOne(x => x.GroupAssignment);
        builder.Navigation(x => x.Associations).HasField("_associations");

        builder.HasDiscriminator<string>("Discriminator")
            .HasValue<GithubSubmission>(nameof(GithubSubmission));
    }
}