using Kysect.Shreks.Core.UserAssociations;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.Seeding.EntityGenerators;

namespace Kysect.Shreks.Seeding.DatabaseSeeders;

public class GithubUserAssociationSeeder : IDatabaseSeeder
{
    private readonly IEntityGenerator<GithubUserAssociation> _generator;

    public GithubUserAssociationSeeder(IEntityGenerator<GithubUserAssociation> generator)
    {
        _generator = generator;
    }

    public void Seed(IShreksDatabaseContext context)
    {
        context.UserAssociations.AddRange(_generator.GeneratedEntities);
    }
}