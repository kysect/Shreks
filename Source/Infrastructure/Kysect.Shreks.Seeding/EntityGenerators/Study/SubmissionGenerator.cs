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
    private readonly IEntityGenerator<Student> _studentGenerator;
    private readonly IEntityGenerator<Assignment> _assignmentGenerator;
    
    public SubmissionGenerator(
        EntityGeneratorOptions<Submission> options,
        IEntityGenerator<Student> studentGenerator,
        IEntityGenerator<Assignment> assignmentGenerator,
        Faker faker) : base(options)
    {
        _faker = faker;
        _assignmentGenerator = assignmentGenerator;
        _studentGenerator = studentGenerator;
    }

    protected override Submission Generate(int index)
    {
        var assignment = _faker.PickRandom<Assignment>(_assignmentGenerator.GeneratedEntities);
        var student = _faker.PickRandom<Student>(_studentGenerator.GeneratedEntities);

        var submission = new Submission(student, assignment, _faker.Date.Future(), _faker.Internet.Url())
        {
            Points = _faker.Random.Double(assignment.MinPoints, assignment.MaxPoints),
            ExtraPoints = _faker.Random.Bool(ChangeOfHavingExtraPoints) ? _faker.Random.Double(0, MaxExtraPoints) : 0
        };
        
        return submission;
    }
}