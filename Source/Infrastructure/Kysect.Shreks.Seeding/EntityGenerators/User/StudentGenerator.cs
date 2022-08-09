using Bogus;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.Seeding.Options;

namespace Kysect.Shreks.Seeding.EntityGenerators.UserGenerators;

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

        var student = new Student
        (
            _faker.Person.FirstName, 
            _faker.Person.UserName, 
            _faker.Person.LastName,
            group
        );
        
        group.AddStudent(student);
                
        return student;
    }
}