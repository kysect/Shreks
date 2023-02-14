using ITMO.Dev.ASAP.Core.SubmissionAssociations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITMO.Dev.ASAP.DataAccess.Configurations.Submissions;

public class SubmissionAssociationConfiguration : IEntityTypeConfiguration<SubmissionAssociation>
{
    public void Configure(EntityTypeBuilder<SubmissionAssociation> builder)
    {
        builder.HasDiscriminator<string>("Discriminator")
            .HasValue<GithubSubmissionAssociation>(nameof(GithubSubmissionAssociation));

        builder.HasIndex("SubmissionId", "Discriminator").IsUnique();
    }
}