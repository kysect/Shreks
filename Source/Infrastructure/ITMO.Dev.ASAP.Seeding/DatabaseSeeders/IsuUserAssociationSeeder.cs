using ITMO.Dev.ASAP.Core.UserAssociations;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.Seeding.EntityGenerators;

namespace ITMO.Dev.ASAP.Seeding.DatabaseSeeders;

public class IsuUserAssociationSeeder : IDatabaseSeeder
{
    private readonly IEntityGenerator<IsuUserAssociation> _generator;

    public IsuUserAssociationSeeder(IEntityGenerator<IsuUserAssociation> generator)
    {
        _generator = generator;
    }

    public void Seed(IDatabaseContext context)
    {
        context.UserAssociations.AddRange(_generator.GeneratedEntities);
    }
}