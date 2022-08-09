using Bogus;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.Seeding.Options;

namespace Kysect.Shreks.Seeding.EntityGenerators;

public class SubmissionGenerator : EntityGeneratorBase<Submission>
{
    private Faker _faker;
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
        var assignmentCount = _assignmentGenerator.GeneratedEntities.Count;
        var studentCount = _studentGenerator.GeneratedEntities.Count;

        var assignmentNumber = index % assignmentCount;
        var studentNumber = index / assignmentCount;

        if (studentNumber >= studentCount)
            throw new IndexOutOfRangeException("Student number more than student count.");

        var assignment = _assignmentGenerator.GeneratedEntities[assignmentNumber];
        var student = _studentGenerator.GeneratedEntities[studentNumber];

        var submission = new Submission(student, assignment, _faker.Date.Future(), _faker.Commerce.Product());
        
        return submission;
    }
}