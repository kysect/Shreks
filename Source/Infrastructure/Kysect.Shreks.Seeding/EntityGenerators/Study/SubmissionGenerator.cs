using Bogus;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.Seeding.Options;

namespace Kysect.Shreks.Seeding.EntityGenerators;

public class SubmissionGenerator : EntityGeneratorBase<Submission>
{
    private const double MaxExtraPoints = 15;
    private const float ChangeOfHavingExtraPoints = 0.1f;

    private readonly Faker _faker;
    private readonly IEntityGenerator<GroupAssignment> _groupAssignmentGenerator;

    public SubmissionGenerator(
        EntityGeneratorOptions<Submission> options,
        Faker faker,
        IEntityGenerator<GroupAssignment> groupAssignmentGenerator) : base(options)
    {
        _faker = faker;
        _groupAssignmentGenerator = groupAssignmentGenerator;
    }

    protected override Submission Generate(int index)
    {
        var groupAssignment = _faker.PickRandom<GroupAssignment>(_groupAssignmentGenerator.GeneratedEntities);
        var assignment = groupAssignment.Assignment;
        var student = _faker.PickRandom<Student>(groupAssignment.Group.Students);

        var submission = new Submission
        (
            student,
            assignment,
            DateOnly.FromDateTime(_faker.Date.Future()),
            _faker.Internet.Url()
        )
        {
            Points = _faker.Random.Double(assignment.MinPoints, assignment.MaxPoints),
            ExtraPoints = _faker.Random.Bool(ChangeOfHavingExtraPoints) ? _faker.Random.Double(0, MaxExtraPoints) : 0,
        };

        return submission;
    }
}