using ITMO.Dev.ASAP.Core.SubjectCourseAssociations;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.Seeding.EntityGenerators;

namespace ITMO.Dev.ASAP.Seeding.DatabaseSeeders;

public class SubjectGroupAssociationSeeder : IDatabaseSeeder
{
    private readonly IEntityGenerator<GithubSubjectCourseAssociation> _generator;

    public SubjectGroupAssociationSeeder(IEntityGenerator<GithubSubjectCourseAssociation> generator)
    {
        _generator = generator;
    }

    public void Seed(IDatabaseContext context)
    {
        context.SubjectCourseAssociations.AddRange(_generator.GeneratedEntities);
    }
}