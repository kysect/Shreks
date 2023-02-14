using Bogus;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.Seeding.Extensions;
using ITMO.Dev.ASAP.Seeding.Options;

namespace ITMO.Dev.ASAP.Seeding.EntityGenerators;

public class AssignmentGenerator : EntityGeneratorBase<Assignment>
{
    private readonly Faker _faker;
    private readonly IEntityGenerator<SubjectCourse> _subjectCourseGenerator;

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
        SubjectCourse? subjectCourse = _faker.PickRandom<SubjectCourse>(_subjectCourseGenerator.GeneratedEntities);

        int assignmentOrder = index + 1;

        return new Assignment(
            _faker.Random.Guid(),
            _faker.Commerce.Product(),
            $"lab-{assignmentOrder}",
            assignmentOrder,
            _faker.Random.Points(0, 5),
            _faker.Random.Points(5, 10),
            subjectCourse);
    }
}