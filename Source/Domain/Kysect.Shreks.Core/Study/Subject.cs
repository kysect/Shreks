using Ardalis.Result;
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
    public IReadOnlyCollection<SubjectCourse> Courses => _courses;

    public Result AddCourse(SubjectCourse course)
    {
        ArgumentNullException.ThrowIfNull(course);

        if (!_courses.Add(course))
            return Result.Error($"Subject already contains course {course}");
        
        return Result.Success();
    }

    public Result RemoveCourse(SubjectCourse course)
    {
        ArgumentNullException.ThrowIfNull(course);

        if (!_courses.Remove(course))
            return Result.Error($"Subject failed to remove course {course}");

        return Result.Success();
    }
}