using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.Seeding.EntityGenerators;

namespace Kysect.Shreks.Seeding.DatabaseSeeders;

public class SubjectSeeder : IDatabaseSeeder
{
    private readonly IEntityGenerator<Subject> _generator;

    public SubjectSeeder(IEntityGenerator<Subject> generator)
    {
        _generator = generator;
    }

    public void Seed(IShreksDatabaseContext context)
        => context.Subjects.AddRange(_generator.GeneratedEntities);
}