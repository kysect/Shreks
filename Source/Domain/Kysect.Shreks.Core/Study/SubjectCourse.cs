using Kysect.Shreks.Core.Exceptions;
using Kysect.Shreks.Core.SubjectCourseAssociations;
using Kysect.Shreks.Core.Users;
using RichEntity.Annotations;

namespace Kysect.Shreks.Core.Study;

public partial class SubjectCourse : IEntity<Guid>
{
    private readonly HashSet<SubjectCourseGroup> _groups;
    private readonly HashSet<Assignment> _assignments;
    private readonly HashSet<SubjectCourseAssociation> _associations;

    public SubjectCourse(Subject subject, Mentor lector) : this(Guid.NewGuid())
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

    public void AddGroup(SubjectCourseGroup group)
    {
        ArgumentNullException.ThrowIfNull(group);

        if (!_groups.Add(group))
            throw new DomainInvalidOperationException($"Group {group} is already assigned to this course");
    }

    public void RemoveGroup(SubjectCourseGroup group)
    {
        ArgumentNullException.ThrowIfNull(group);

        if (!_groups.Remove(group))
            throw new DomainInvalidOperationException($"Group {group} is not assigned to this course");
    }

    public void AddAssignment(Assignment assignment)
    {
        ArgumentNullException.ThrowIfNull(assignment);

        if (!_assignments.Add(assignment))
            throw new DomainInvalidOperationException($"Assignment {assignment} is already assigned to this course");
    }

    public void RemoveAssignment(Assignment assignment)
    {
        ArgumentNullException.ThrowIfNull(assignment);

        if (!_assignments.Remove(assignment))
            throw new DomainInvalidOperationException($"Assignment {assignment} is not assigned to this course");
    }
    
    public void AddAssociation(SubjectCourseAssociation association)
    {
        ArgumentNullException.ThrowIfNull(association);

        if (!_associations.Add(association))
            throw new DomainInvalidOperationException($"Association {association} is already assigned to this course");
    }
    
    public void RemoveAssociation(SubjectCourseAssociation association)
    {
        ArgumentNullException.ThrowIfNull(association);

        if (!_associations.Remove(association))
            throw new DomainInvalidOperationException($"Association {association} is not assigned to this course");
    }
}