using Bogus;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Seeding.Options;

namespace Kysect.Shreks.Seeding.EntityGenerators.StudyGenerators;

public class SubmissionGenerator : EntityGeneratorBase<Submission>
{
    private readonly IEntityGenerator<StudentAssignment> _studentAssignmentGenerator;
    private Faker _faker;
    
    public SubmissionGenerator(
        EntityGeneratorOptions<Submission> options,
        IEntityGenerator<StudentAssignment> studentAssignmentGenerator,
        Faker faker) 
        : base(options)
    {
        _studentAssignmentGenerator = studentAssignmentGenerator;
        _faker = faker;
    }

    protected override Submission Generate(int index)
    {
        var count = _studentAssignmentGenerator.GeneratedEntities.Count;
        var number = _faker.Random.Number(0, count - 1);

        var studentAssignment = _studentAssignmentGenerator.GeneratedEntities[number];

        var submission = new Submission(studentAssignment);
        
        studentAssignment.AddSubmission(submission);

        return submission;
    }
}