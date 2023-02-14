using ITMO.Dev.ASAP.Core.UserAssociations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITMO.Dev.ASAP.DataAccess.Configurations.UserAssociations;

public class IsuUserAssociationConfiguration : IEntityTypeConfiguration<IsuUserAssociation>
{
    public void Configure(EntityTypeBuilder<IsuUserAssociation> builder)
    {
        builder.HasIndex(x => x.UniversityId).IsUnique();
    }
}