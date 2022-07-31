using Bogus;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.Seeding.Options;

namespace Kysect.Shreks.Seeding.EntityGenerators.StudyGenerators;

public class StudentAssignmentGenerator : EntityGeneratorBase<StudentAssignment>
{
    private readonly IEntityGenerator<Student> _studentGenerator;
    private readonly IEntityGenerator<Assignment> _assignmentGenerator;
    private readonly Faker _faker;
    
    public StudentAssignmentGenerator(
        EntityGeneratorOptions<StudentAssignment> options,
        IEntityGenerator<Student> studentGenerator,
        IEntityGenerator<Assignment> assignmentGenerator,
        Faker faker) 
        : base(options)
    {
        _studentGenerator = studentGenerator;
        _assignmentGenerator = assignmentGenerator;
        _faker = faker;
    }

    protected override StudentAssignment Generate(int index)
    {
        var assignmentCount = _assignmentGenerator.GeneratedEntities.Count;
        var assignmentNumber = _faker.Random.Number(0, assignmentCount - 1);
        var assignment = _assignmentGenerator.GeneratedEntities[assignmentNumber];

        var studentCount = _studentGenerator.GeneratedEntities.Count;
        var studentNumber = _faker.Random.Number(0, studentCount - 1);
        var student = _studentGenerator.GeneratedEntities[studentNumber];

        return new StudentAssignment(student, assignment);
    }
}