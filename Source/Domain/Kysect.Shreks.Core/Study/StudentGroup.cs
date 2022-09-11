using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.Queue.Filters;
using Kysect.Shreks.Core.Users;
using RichEntity.Annotations;

namespace Kysect.Shreks.Core.Study;

public partial class StudentGroup : IEntity<Guid>
{
    private readonly HashSet<Student> _students;

    public StudentGroup(string name)
        : this(Guid.NewGuid())
    {
        ArgumentNullException.ThrowIfNull(name);

        Name = name;
        _students = new HashSet<Student>();
    }

    public string Name { get; set; }
    public virtual IReadOnlyCollection<Student> Students => _students;

    public override string ToString()
        => Name;

    public void AddStudent(Student student)
    {
        ArgumentNullException.ThrowIfNull(student);

        if (!_students.Add(student))
            throw new DomainInvalidOperationException($"Student {student} already a member of group {this}");
    }

    public void RemoveStudent(Student student)
    {
        ArgumentNullException.ThrowIfNull(student);

        if (!_students.Remove(student))
            throw new DomainInvalidOperationException($"Removing student {student} from group {this} failed");
    }
}