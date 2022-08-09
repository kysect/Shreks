using Kysect.Shreks.Application.Abstractions.DataAccess;
using Kysect.Shreks.Core.UserAssociations;
using Kysect.Shreks.Seeding.EntityGenerators;

namespace Kysect.Shreks.Seeding.DatabaseSeeders;

public class IsuUserAssociationSeeder : IDatabaseSeeder
{
    private readonly IEntityGenerator<IsuUserAssociation> _generator;

    public IsuUserAssociationSeeder(IEntityGenerator<IsuUserAssociation> generator)
    {
        _generator = generator;
    }

    public void Seed(IShreksDatabaseContext context)
        => context.UserAssociations.AddRange(_generator.GeneratedEntities);
}