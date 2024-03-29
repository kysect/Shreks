using Kysect.Shreks.Core.SubmissionAssociations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kysect.Shreks.DataAccess.Configurations.Submissions;

public class SubmissionAssociationConfiguration : IEntityTypeConfiguration<SubmissionAssociation>
{
    public void Configure(EntityTypeBuilder<SubmissionAssociation> builder)
    {
        builder.HasDiscriminator<string>("Discriminator")
            .HasValue<GithubSubmissionAssociation>(nameof(GithubSubmissionAssociation));

        builder.HasIndex("SubmissionId", "Discriminator").IsUnique();
    }
}