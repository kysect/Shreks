using Kysect.Shreks.Core.SubjectCourseAssociations;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.Seeding.EntityGenerators;

namespace Kysect.Shreks.Seeding.DatabaseSeeders;

public class SubjectGroupAssociationSeeder : IDatabaseSeeder
{
    private readonly IEntityGenerator<GithubSubjectCourseAssociation> _generator;

    public SubjectGroupAssociationSeeder(IEntityGenerator<GithubSubjectCourseAssociation> generator)
    {
        _generator = generator;
    }

    public void Seed(IShreksDatabaseContext context)
    {
        context.SubjectCourseAssociations.AddRange(_generator.GeneratedEntities);
    }
}