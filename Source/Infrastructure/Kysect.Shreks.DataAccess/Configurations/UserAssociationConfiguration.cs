using Kysect.Shreks.Core.UserAssociations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kysect.Shreks.DataAccess.Configurations;

public class UserAssociationConfiguration : IEntityTypeConfiguration<UserAssociation>
{
    public void Configure(EntityTypeBuilder<UserAssociation> builder)
    {
        builder.HasDiscriminator<string>("UserAssociationType")
            .HasValue<IsuUserAssociation>(nameof(IsuUserAssociation))
            .HasValue<GithubUserAssociation>(nameof(GithubUserAssociation));
    }
}