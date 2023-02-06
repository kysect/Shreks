using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.Seeding.EntityGenerators;

namespace Kysect.Shreks.Seeding.DatabaseSeeders;

public class SubjectCourseGroupSeeder : IDatabaseSeeder
{
    private readonly IEntityGenerator<SubjectCourseGroup> _generator;

    public int Priority => 1;

    public SubjectCourseGroupSeeder(IEntityGenerator<SubjectCourseGroup> generator)
    {
        _generator = generator;
    }

    public void Seed(IShreksDatabaseContext context)
    {
        context.SubjectCourseGroups.AddRange(_generator.GeneratedEntities);
    }
}