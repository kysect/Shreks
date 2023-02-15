using ITMO.Dev.ASAP.Core.Users;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.Seeding.EntityGenerators;

namespace ITMO.Dev.ASAP.Seeding.DatabaseSeeders;

public class StudentSeeder : IDatabaseSeeder
{
    private readonly IEntityGenerator<Student> _generator;

    public StudentSeeder(IEntityGenerator<Student> generator)
    {
        _generator = generator;
    }

    public int Priority => 1;

    public void Seed(IDatabaseContext context)
    {
        context.Students.AddRange(_generator.GeneratedEntities);
    }
}