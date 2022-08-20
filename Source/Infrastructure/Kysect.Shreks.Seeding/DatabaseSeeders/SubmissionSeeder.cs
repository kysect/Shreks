using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.Seeding.EntityGenerators;

namespace Kysect.Shreks.Seeding.DatabaseSeeders;

public class SubmissionSeeder : IDatabaseSeeder
{
    private readonly IEntityGenerator<Submission> _generator;

    public SubmissionSeeder(IEntityGenerator<Submission> generator)
    {
        _generator = generator;
    }

    public void Seed(IShreksDatabaseContext context)
        => context.Submissions.AddRange(_generator.GeneratedEntities);
}