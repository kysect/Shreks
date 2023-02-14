using ITMO.Dev.ASAP.Core.UserAssociations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITMO.Dev.ASAP.DataAccess.Configurations.UserAssociations;

public class GithubUserAssociationConfiguration : IEntityTypeConfiguration<GithubUserAssociation>
{
    public void Configure(EntityTypeBuilder<GithubUserAssociation> builder)
    {
        builder.HasIndex(x => x.GithubUsername).IsUnique();
    }
}