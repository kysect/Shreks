using Bogus;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.Seeding.Options;

namespace Kysect.Shreks.Seeding.EntityGenerators;

public class StudentGenerator : EntityGeneratorBase<Student>
{
    private readonly IEntityGenerator<StudentGroup> _studentGroupGenerator;
    private readonly IEntityGenerator<User> _userGenerator;
    private readonly Faker _faker;

    public StudentGenerator(
        EntityGeneratorOptions<Student> options,
        IEntityGenerator<StudentGroup> studentGroupGenerator,
        Faker faker,
        IEntityGenerator<User> userGenerator) : base(options)
    {
        _studentGroupGenerator = studentGroupGenerator;
        _faker = faker;
        _userGenerator = userGenerator;
    }

    protected override Student Generate(int index)
    {
        if (index >= _userGenerator.GeneratedEntities.Count)
            throw new IndexOutOfRangeException("Student count is greater than count of users.");
        
        var groupCount = _studentGroupGenerator.GeneratedEntities.Count;
        var groupNumber = _faker.Random.Number(0, groupCount - 1);

        StudentGroup group = _studentGroupGenerator.GeneratedEntities[groupNumber];

        var user = _userGenerator.GeneratedEntities[index];

        var student = new Student(user, group);

        group.AddStudent(student);

        return student;
    }
}