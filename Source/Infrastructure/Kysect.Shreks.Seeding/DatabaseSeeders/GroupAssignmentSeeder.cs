using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.Seeding.EntityGenerators;

namespace Kysect.Shreks.Seeding.DatabaseSeeders;

public class GroupAssignmentSeeder : IDatabaseSeeder
{
    private readonly IEntityGenerator<GroupAssignment> _generator;

    public GroupAssignmentSeeder(IEntityGenerator<GroupAssignment> generator)
    {
        _generator = generator;
    }

    public void Seed(IShreksDatabaseContext context)
        => context.GroupAssignments.AddRange(_generator.GeneratedEntities);
}