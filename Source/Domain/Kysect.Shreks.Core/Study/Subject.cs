using Kysect.Shreks.Common.Exceptions;
using RichEntity.Annotations;

namespace Kysect.Shreks.Core.Study;

public partial class Subject : IEntity<Guid>
{
    private readonly HashSet<SubjectCourse> _courses;

    public Subject(string title) : this(Guid.NewGuid())
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