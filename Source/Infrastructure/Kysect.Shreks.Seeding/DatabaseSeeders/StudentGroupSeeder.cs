using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.Seeding.EntityGenerators;

namespace Kysect.Shreks.Seeding.DatabaseSeeders;

public class StudentGroupSeeder : IDatabaseSeeder
{
    private readonly IEntityGenerator<StudentGroup> _generator;

    public StudentGroupSeeder(IEntityGenerator<StudentGroup> generator)
    {
        _generator = generator;
    }

    public void Seed(IShreksDatabaseContext context)
    {
        context.StudentGroups.AddRange(_generator.GeneratedEntities);
    }
}