using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.Seeding.EntityGenerators;

namespace Kysect.Shreks.Seeding.DatabaseSeeders;

public class GithubSubmissionSeeder : IDatabaseSeeder
{
    private readonly IEntityGenerator<GithubSubmission> _generator;

    public GithubSubmissionSeeder(IEntityGenerator<GithubSubmission> generator)
    {
        _generator = generator;
    }

    public void Seed(IShreksDatabaseContext context)
    {
        context.Submissions.AddRange(_generator.GeneratedEntities);
    }
}