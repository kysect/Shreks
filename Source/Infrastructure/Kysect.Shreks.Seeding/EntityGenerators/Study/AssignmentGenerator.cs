using Bogus;
using Kysect.Shreks.Core.DeadlinePolicies;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Seeding.Options;

namespace Kysect.Shreks.Seeding.EntityGenerators;

public class AssignmentGenerator : EntityGeneratorBase<Assignment>
{
    private readonly IEntityGenerator<DeadlinePolicy> _deadlinePolicyGenerator;
    private readonly IEntityGenerator<SubjectCourse> _subjectCourseGenerator;
    private readonly Faker _faker;

    public AssignmentGenerator(
        EntityGeneratorOptions<Assignment> options,
        IEntityGenerator<DeadlinePolicy> deadlinePolicyGenerator,
        IEntityGenerator<SubjectCourse> subjectCourseGenerator,
        Faker faker)
        : base(options)
    {
        _deadlinePolicyGenerator = deadlinePolicyGenerator;
        _subjectCourseGenerator = subjectCourseGenerator;
        _faker = faker;
    }

    protected override Assignment Generate(int index)
    {
        var deadlineCount = _faker.Random.Int(0, _deadlinePolicyGenerator.GeneratedEntities.Count);

        IEnumerable<DeadlinePolicy> deadlines = Enumerable.Range(0, deadlineCount)
            .Select(_ => _faker.Random.Int(0, _deadlinePolicyGenerator.GeneratedEntities.Count - 1))
            .Select(i => _deadlinePolicyGenerator.GeneratedEntities[i])
            .Distinct();

        var assignment = new Assignment
        (
            _faker.Commerce.Product(),
            (index + 1).ToString(),
            _faker.Random.Double(0, 5),
            _faker.Random.Double(5, 10)
        );

        foreach (var deadline in deadlines)
        {
            assignment.AddDeadlinePolicy(deadline);
        }

        var subjectCourse = _faker.PickRandom<SubjectCourse>(_subjectCourseGenerator.GeneratedEntities);
        subjectCourse.AddAssignment(assignment);

        return assignment;
    }
}