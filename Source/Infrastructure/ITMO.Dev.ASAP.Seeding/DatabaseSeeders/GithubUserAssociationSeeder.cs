using ITMO.Dev.ASAP.Core.UserAssociations;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.Seeding.EntityGenerators;

namespace ITMO.Dev.ASAP.Seeding.DatabaseSeeders;

public class GithubUserAssociationSeeder : IDatabaseSeeder
{
    private readonly IEntityGenerator<GithubUserAssociation> _generator;

    public GithubUserAssociationSeeder(IEntityGenerator<GithubUserAssociation> generator)
    {
        _generator = generator;
    }

    public void Seed(IDatabaseContext context)
    {
        context.UserAssociations.AddRange(_generator.GeneratedEntities);
    }
}