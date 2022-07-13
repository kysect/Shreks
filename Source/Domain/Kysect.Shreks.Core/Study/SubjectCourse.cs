using Ardalis.Result;
using Kysect.Shreks.Core.SubjectCourseAssociations;
using Kysect.Shreks.Core.Users;
using RichEntity.Annotations;

namespace Kysect.Shreks.Core.Study;

public partial class SubjectCourse : IEntity<Guid>
{
    private readonly HashSet<SubjectCourseGroup> _groups;
    private readonly HashSet<Assignment> _assignments;
    private readonly HashSet<SubjectCourseAssociation> _associations;

    public SubjectCourse(Subject subject, Mentor lector)
    {
        Subject = subject;
        Lector = lector;

        _groups = new HashSet<SubjectCourseGroup>();
        _assignments = new HashSet<Assignment>();
        _associations = new HashSet<SubjectCourseAssociation>();
    }

    public virtual Subject Subject { get; protected init; }
    public virtual Mentor Lector { get; protected init; }
    public virtual IReadOnlyCollection<SubjectCourseGroup> Groups => _groups;
    public virtual IReadOnlyCollection<Assignment> Assignments => _assignments;
    public virtual IReadOnlyCollection<SubjectCourseAssociation> Associations => _associations;

    public Result AddGroup(SubjectCourseGroup group)
    {
        ArgumentNullException.ThrowIfNull(group);

        if (!_groups.Add(group))
            return Result.Error($"Group {group} is already assigned to this course");
        
        return Result.Success();
    }

    public Result RemoveGroup(SubjectCourseGroup group)
    {
        ArgumentNullException.ThrowIfNull(group);

        if (!_groups.Remove(group))
            return Result.Error($"Group {group} is not assigned to this course");

        return Result.Success();
    }

    public Result AddAssignment(Assignment assignment)
    {
        ArgumentNullException.ThrowIfNull(assignment);

        if (!_assignments.Add(assignment))
            return Result.Error($"Assignment {assignment} is already assigned to this course");
        
        return Result.Success();
    }

    public Result RemoveAssignment(Assignment assignment)
    {
        ArgumentNullException.ThrowIfNull(assignment);

        if (!_assignments.Remove(assignment))
            return Result.Error($"Assignment {assignment} is not assigned to this course");

        return Result.Success();
    }
    
    public Result AddAssociation(SubjectCourseAssociation association)
    {
        ArgumentNullException.ThrowIfNull(association);

        if (!_associations.Add(association))
            return Result.Error($"Association {association} is already assigned to this course");
        
        return Result.Success();
    }
    
    public Result RemoveAssociation(SubjectCourseAssociation association)
    {
        ArgumentNullException.ThrowIfNull(association);

        if (!_associations.Remove(association))
            return Result.Error($"Association {association} is not assigned to this course");

        return Result.Success();
    }
}