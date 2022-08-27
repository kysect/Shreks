using Kysect.Shreks.Core.UserAssociations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kysect.Shreks.DataAccess.Configurations.Users;

public class IsuUserAssociationConfiguration : IEntityTypeConfiguration<IsuUserAssociation>
{
    public void Configure(EntityTypeBuilder<IsuUserAssociation> builder)
    {
        builder.HasIndex(x => x.UniversityId).IsUnique();
    }
}