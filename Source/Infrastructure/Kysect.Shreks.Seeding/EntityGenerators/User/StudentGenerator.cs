using Bogus;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.Seeding.Options;

namespace Kysect.Shreks.Seeding.EntityGenerators;

public class StudentGenerator : EntityGeneratorBase<Student>
{
    private readonly IEntityGenerator<StudentGroup> _studentGroupGenerator;
    private readonly Faker _faker;

    public StudentGenerator(
        EntityGeneratorOptions<Student> options,
        IEntityGenerator<StudentGroup> studentGroupGenerator,
        Faker faker) : base(options)
    {
        _studentGroupGenerator = studentGroupGenerator;
        _faker = faker;
    }
    
    protected override Student Generate(int index)
    {
        var groupCount = _studentGroupGenerator.GeneratedEntities.Count;
        var groupNumber = _faker.Random.Number(0, groupCount - 1);
        
        StudentGroup group = _studentGroupGenerator.GeneratedEntities[groupNumber];

        var middleName = _faker.Random.Bool(0.95f)
            ? _faker.Name.FirstName()
            : string.Empty;

        var student = new Student
        (
            _faker.Name.FirstName(),
            middleName,
            _faker.Name.LastName(),
            group
        );
        
        group.AddStudent(student);
                
        return student;
    }
}