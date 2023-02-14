using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.Core.Users;
using ITMO.Dev.ASAP.Seeding.Options;

namespace ITMO.Dev.ASAP.Seeding.EntityGenerators;

public class StudentGenerator : EntityGeneratorBase<Student>
{
    private readonly IEntityGenerator<StudentGroup> _studentGroupGenerator;
    private readonly IEntityGenerator<User> _userGenerator;

    public StudentGenerator(
        EntityGeneratorOptions<Student> options,
        IEntityGenerator<StudentGroup> studentGroupGenerator,
        IEntityGenerator<User> userGenerator)
        : base(options)
    {
        _studentGroupGenerator = studentGroupGenerator;
        _userGenerator = userGenerator;
    }

    protected override Student Generate(int index)
    {
        if (index >= _userGenerator.GeneratedEntities.Count)
            throw new IndexOutOfRangeException("Student count is greater than count of users.");

        User user = _userGenerator.GeneratedEntities[index];

        StudentGroup[] groups = _studentGroupGenerator.GeneratedEntities
            .Where(x => x.Students.Any(student => student.User.Equals(user)) is false)
            .ToArray();

        int groupNumber = index % groups.Length;
        StudentGroup group = groups[groupNumber];

        return new Student(user, group);
    }
}