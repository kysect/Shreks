using Kysect.Shreks.Core.SubjectCourseAssociations;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.Seeding.EntityGenerators;

namespace Kysect.Shreks.Seeding.DatabaseSeeders;

public class SubjectCourseAssociationSeeder : IDatabaseSeeder
{
    private readonly IEntityGenerator<SubjectCourseAssociation> _generator;

    public SubjectCourseAssociationSeeder(IEntityGenerator<SubjectCourseAssociation> generator)
    {
        _generator = generator;
    }

    public void Seed(IShreksDatabaseContext context)
        => context.SubjectCourseAssociations.AddRange(_generator.GeneratedEntities);
}