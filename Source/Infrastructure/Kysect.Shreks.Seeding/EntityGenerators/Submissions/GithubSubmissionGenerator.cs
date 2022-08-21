using Bogus;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.SubmissionAssociations;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.Core.ValueObject;
using Kysect.Shreks.Seeding.Extensions;
using Kysect.Shreks.Seeding.Options;

namespace Kysect.Shreks.Seeding.EntityGenerators.Submissions;

public class GithubSubmissionGenerator : EntityGeneratorBase<GithubSubmission>
{
    private const double MaxExtraPoints = 15;
    private const float ChangeOfHavingExtraPoints = 0.1f;

    private readonly Faker _faker;
    private readonly IEntityGenerator<Student> _studentGenerator;
    private readonly IEntityGenerator<GroupAssignment> _assignmentGenerator;

    public GithubSubmissionGenerator(
        EntityGeneratorOptions<GithubSubmission> options,
        IEntityGenerator<Student> studentGenerator,
        IEntityGenerator<GroupAssignment> assignmentGenerator,
        Faker faker)
        : base(options)
    {
        _faker = faker;
        _assignmentGenerator = assignmentGenerator;
        _studentGenerator = studentGenerator;
    }

    protected override GithubSubmission Generate(int index)
    {
        var assignment = _faker.PickRandom<GroupAssignment>(_assignmentGenerator.GeneratedEntities);
        var student = _faker.PickRandom<Student>(_studentGenerator.GeneratedEntities);

        var submission = new GithubSubmission
        (
            student,
            assignment,
            DateOnly.FromDateTime(_faker.Date.Future()),
            _faker.Internet.Url(),
            _faker.Company.CompanyName(),
            _faker.Commerce.ProductName(),
            _faker.Random.Long(0, 100)
        )
        {
            Rating = _faker.Random.Fraction(),
            ExtraPoints = _faker.Random.Bool(ChangeOfHavingExtraPoints)
                ? _faker.Random.Points(0, MaxExtraPoints)
                : Points.None
        };

        return submission;
    }
}