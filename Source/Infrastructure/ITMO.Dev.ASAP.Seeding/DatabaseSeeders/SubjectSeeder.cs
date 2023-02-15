using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.Seeding.EntityGenerators;

namespace ITMO.Dev.ASAP.Seeding.DatabaseSeeders;

public class SubjectSeeder : IDatabaseSeeder
{
    private readonly IEntityGenerator<Subject> _generator;

    public SubjectSeeder(IEntityGenerator<Subject> generator)
    {
        _generator = generator;
    }

    public void Seed(IDatabaseContext context)
    {
        context.Subjects.AddRange(_generator.GeneratedEntities);
    }
}