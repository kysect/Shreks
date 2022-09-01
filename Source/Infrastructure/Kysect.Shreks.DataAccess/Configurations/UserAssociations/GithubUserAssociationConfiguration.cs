using Kysect.Shreks.Core.UserAssociations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kysect.Shreks.DataAccess.Configurations.UserAssociations;

public class GithubUserAssociationConfiguration : IEntityTypeConfiguration<GithubUserAssociation>
{
    public void Configure(EntityTypeBuilder<GithubUserAssociation> builder)
    {
        builder.HasIndex(x => x.GithubUsername).IsUnique();
    }
}