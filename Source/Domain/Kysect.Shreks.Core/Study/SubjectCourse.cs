using Ardalis.Result;
using Kysect.Shreks.Core.SubjectCourseAssociations;
using Kysect.Shreks.Core.Users;
using RichEntity.Annotations;

namespace Kysect.Shreks.Core.Study;

public partial class SubjectCourse : IEntity<Guid>
{
    private readonly List<SubjectCourseGroup> _groups;
    private readonly List<Assignment> _assignments;
    private readonly List<SubjectCourseAssociation> _associations;

    public SubjectCourse(Subject subject, Mentor lector)
    {
        Subject = subject;
        Lector = lector;

        _groups = new List<SubjectCourseGroup>();
        _assignments = new List<Assignment>();
        _associations = new List<SubjectCourseAssociation>();
    }

    public virtual Subject Subject { get; protected init; }
    public virtual Mentor Lector { get; protected init; }
    public virtual IReadOnlyCollection<SubjectCourseGroup> Groups => _groups.AsReadOnly();
    public virtual IReadOnlyCollection<Assignment> Assignments => _assignments.AsReadOnly();
    public virtual IReadOnlyCollection<SubjectCourseAssociation> Associations => _associations.AsReadOnly();

    public Result AddGroup(SubjectCourseGroup group)
    {
        ArgumentNullException.ThrowIfNull(group);

        if (_groups.Contains(group))
            return Result.Error($"Group {group} is already assigned to this course");

        _groups.Add(group);
        return Result.Success();
    }

    public Result RemoveGroup(SubjectCourseGroup group)
    {
        ArgumentNullException.ThrowIfNull(group);

        if (!_groups.Contains(group))
            return Result.Error($"Group {group} is not assigned to this course");

        if (!_groups.Remove(group))
            return Result.Error($"Group {group} is not assigned to this course");

        return Result.Success();
    }

    public Result AddAssignment(Assignment assignment)
    {
        ArgumentNullException.ThrowIfNull(assignment);

        if (_assignments.Contains(assignment))
            return Result.Error($"Assignment {assignment} is already assigned to this course");

        _assignments.Add(assignment);
        return Result.Success();
    }

    public Result RemoveAssignment(Assignment assignment)
    {
        ArgumentNullException.ThrowIfNull(assignment);

        if (!_assignments.Contains(assignment))
            return Result.Error($"Assignment {assignment} is not assigned to this course");

        if (!_assignments.Remove(assignment))
            return Result.Error($"Assignment {assignment} is not assigned to this course");

        return Result.Success();
    }
    
    public Result AddAssociation(SubjectCourseAssociation association)
    {
        ArgumentNullException.ThrowIfNull(association);

        if (_associations.Contains(association))
            return Result.Error($"Association {association} is already assigned to this course");

        _associations.Add(association);
        return Result.Success();
    }
    
    public Result RemoveAssociation(SubjectCourseAssociation association)
    {
        ArgumentNullException.ThrowIfNull(association);

        if (!_associations.Contains(association))
            return Result.Error($"Association {association} is not assigned to this course");

        if (!_associations.Remove(association))
            return Result.Error($"Association {association} is not assigned to this course");

        return Result.Success();
    }
}