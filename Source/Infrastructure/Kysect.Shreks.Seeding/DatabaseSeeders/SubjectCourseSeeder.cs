using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.Seeding.EntityGenerators;

namespace Kysect.Shreks.Seeding.DatabaseSeeders;

public class SubjectCourseSeeder : IDatabaseSeeder
{
    private readonly IEntityGenerator<SubjectCourse> _generator;

    public SubjectCourseSeeder(IEntityGenerator<SubjectCourse> generator)
    {
        _generator = generator;
    }

    public void Seed(IShreksDatabaseContext context)
    {
        context.SubjectCourses.AddRange(_generator.GeneratedEntities);
    }
}