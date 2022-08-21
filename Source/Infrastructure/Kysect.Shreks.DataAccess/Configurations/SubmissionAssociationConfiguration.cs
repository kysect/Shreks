using Kysect.Shreks.Core.SubmissionAssociations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kysect.Shreks.DataAccess.Configurations;

public class SubmissionAssociationConfiguration : IEntityTypeConfiguration<SubmissionAssociation>
{
    public void Configure(EntityTypeBuilder<SubmissionAssociation> builder)
    {
        builder.HasDiscriminator<string>("Discriminator")
            .HasValue<GithubSubmissionAssociation>(nameof(GithubSubmissionAssociation));
    }
}