using Kysect.Shreks.Application.Abstractions.DataAccess;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.Seeding.EntityGenerators;

namespace Kysect.Shreks.Seeding.DatabaseSeeders;

public class StudentSeeder : IDatabaseSeeder
{
    private readonly IEntityGenerator<Student> _generator;

    public StudentSeeder(IEntityGenerator<Student> generator)
    {
        _generator = generator;
    }

    public void Seed(IShreksDatabaseContext context)
        => context.Students.AddRange(_generator.GeneratedEntities);
}