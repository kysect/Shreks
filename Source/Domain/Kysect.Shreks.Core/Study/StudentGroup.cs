using Ardalis.Result;
using Kysect.Shreks.Core.Users;
using RichEntity.Annotations;

namespace Kysect.Shreks.Core.Study;

public partial class StudentGroup : IEntity<Guid>
{
    private readonly List<Student> _students;

    public StudentGroup(string name)
        : this(Guid.NewGuid())
    {
        ArgumentNullException.ThrowIfNull(name);

        Name = name;
        _students = new List<Student>();
    }

    public string Name { get; protected init; }
    public virtual IReadOnlyCollection<Student> Students => _students.AsReadOnly();

    public override string ToString()
        => Name;

    public Result AddStudent(Student student)
    {
        ArgumentNullException.ThrowIfNull(student);

        if (_students.Contains(student))
            return Result.Error($"Student {student} already a member of group {this}.");

        _students.Add(student);
        return Result.Success();
    }

    public Result RemoveStudent(Student student)
    {
        ArgumentNullException.ThrowIfNull(student);

        if (!_students.Contains(student))
            return Result.Error($"Student {student} is not a member of group {this}.");

        if (!_students.Remove(student))
            return Result.Error($"Removing student {student} from group {this} failed.");

        return Result.Success();
    }
}