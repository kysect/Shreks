using Bogus;
using Kysect.Shreks.Core.DeadlinePolicies;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Seeding.Extensions;
using Kysect.Shreks.Seeding.Options;

namespace Kysect.Shreks.Seeding.EntityGenerators;

public class AssignmentGenerator : EntityGeneratorBase<Assignment>
{
    private readonly IEntityGenerator<SubjectCourse> _subjectCourseGenerator;
    private readonly Faker _faker;

    public AssignmentGenerator(
        EntityGeneratorOptions<Assignment> options,
        IEntityGenerator<SubjectCourse> subjectCourseGenerator,
        Faker faker)
        : base(options)
    {
        _subjectCourseGenerator = subjectCourseGenerator;
        _faker = faker;
    }

    protected override Assignment Generate(int index)
    {

        var subjectCourse = _faker.PickRandom<SubjectCourse>(_subjectCourseGenerator.GeneratedEntities);

        var assignmentOrder = index + 1;

        var assignment = new Assignment
        (
            _faker.Commerce.Product(),
            $"lab-{assignmentOrder}",
            assignmentOrder,
            _faker.Random.Points(0, 5),
            _faker.Random.Points(5, 10),
            subjectCourse
        );

        subjectCourse.AddAssignment(assignment);

        return assignment;
    }
}