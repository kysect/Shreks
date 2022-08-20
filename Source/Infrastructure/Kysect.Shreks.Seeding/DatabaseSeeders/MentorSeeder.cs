using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.Seeding.EntityGenerators;

namespace Kysect.Shreks.Seeding.DatabaseSeeders;

public class MentorSeeder : IDatabaseSeeder
{
    private readonly IEntityGenerator<Mentor> _generator;

    public MentorSeeder(IEntityGenerator<Mentor> generator)
    {
        _generator = generator;
    }

    public void Seed(IShreksDatabaseContext context)
        => context.Mentors.AddRange(_generator.GeneratedEntities);
}