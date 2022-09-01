using Kysect.Shreks.Core.UserAssociations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kysect.Shreks.DataAccess.Configurations.UserAssociations;

public class UserAssociationConfiguration : IEntityTypeConfiguration<UserAssociation>
{
    public void Configure(EntityTypeBuilder<UserAssociation> builder)
    {
        builder.HasDiscriminator<string>("Discriminator")
            .HasValue<IsuUserAssociation>(nameof(IsuUserAssociation))
            .HasValue<GithubUserAssociation>(nameof(GithubUserAssociation));

        builder.HasIndex("UserId", "Discriminator").IsUnique();
    }
}