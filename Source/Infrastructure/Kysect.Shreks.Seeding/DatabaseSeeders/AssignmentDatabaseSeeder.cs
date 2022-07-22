using Kysect.Shreks.Application.Abstractions.DataAccess;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Seeding.EntityGenerators;

namespace Kysect.Shreks.Seeding.DatabaseSeeders;

public class AssignmentDatabaseSeeder : IDatabaseSeeder
{
    private readonly IEntityGenerator<Assignment> _generator;

    public AssignmentDatabaseSeeder(IEntityGenerator<Assignment> generator)
    {
        _generator = generator;
    }

    public void Seed(IShreksDatabaseContext context)
        => context.Assignments.AddRange(_generator.GeneratedEntities);
}