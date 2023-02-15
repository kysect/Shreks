using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.Seeding.EntityGenerators;

namespace ITMO.Dev.ASAP.Seeding.DatabaseSeeders;

public class StudentGroupSeeder : IDatabaseSeeder
{
    private readonly IEntityGenerator<StudentGroup> _generator;

    public StudentGroupSeeder(IEntityGenerator<StudentGroup> generator)
    {
        _generator = generator;
    }

    public void Seed(IDatabaseContext context)
    {
        context.StudentGroups.AddRange(_generator.GeneratedEntities);
    }
}