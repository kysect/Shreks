using Kysect.Shreks.Application.Abstractions.DataAccess;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Seeding.EntityGenerators;

namespace Kysect.Shreks.Seeding.DatabaseSeeders;

public class SubjectCourseGroupSeeder : IDatabaseSeeder
{
    private readonly IEntityGenerator<SubjectCourseGroup> _generator;

    public SubjectCourseGroupSeeder(IEntityGenerator<SubjectCourseGroup> generator)
    {
        _generator = generator;
    }

    public void Seed(IShreksDatabaseContext context)
        => context.SubjectCourseGroups.AddRange(_generator.GeneratedEntities);
}