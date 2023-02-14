using ITMO.Dev.ASAP.Common.Exceptions;
using RichEntity.Annotations;

namespace ITMO.Dev.ASAP.Core.Study;

public partial class Subject : IEntity<Guid>
{
    private readonly HashSet<SubjectCourse> _courses;

    public Subject(Guid id, string title) : this(id)
    {
        ArgumentNullException.ThrowIfNull(title);

        Title = title;
        _courses = new HashSet<SubjectCourse>();
    }

    public string Title { get; set; }

    public virtual IReadOnlyCollection<SubjectCourse> Courses => _courses;

    public void AddCourse(SubjectCourse course)
    {
        ArgumentNullException.ThrowIfNull(course);

        if (!_courses.Add(course))
            throw new DomainInvalidOperationException($"Subject already contains course {course}");
    }

    public void RemoveCourse(SubjectCourse course)
    {
        ArgumentNullException.ThrowIfNull(course);

        if (!_courses.Remove(course))
            throw new DomainInvalidOperationException($"Subject failed to remove course {course}");
    }
}