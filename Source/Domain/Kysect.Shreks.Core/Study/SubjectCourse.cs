using Kysect.Shreks.Core.Exceptions;
using Kysect.Shreks.Core.Models;
using Kysect.Shreks.Core.Queue;
using Kysect.Shreks.Core.Queue.Evaluators;
using Kysect.Shreks.Core.Queue.Filters;
using Kysect.Shreks.Core.SubjectCourseAssociations;
using Kysect.Shreks.Core.Users;
using RichEntity.Annotations;

namespace Kysect.Shreks.Core.Study;

public partial class SubjectCourse : IEntity<Guid>
{
    private readonly HashSet<SubjectCourseGroup> _groups;
    private readonly HashSet<Assignment> _assignments;
    private readonly HashSet<SubjectCourseAssociation> _associations;
    private readonly HashSet<Mentor> _mentors;
    
    // TODO: Remove when .NET 7 is released
    protected virtual IReadOnlyCollection<SubmissionQueueFilter> Filters { get; init; }

    public SubjectCourse(Subject subject, string name) : this(Guid.NewGuid())
    {
        Subject = subject;
        Name = name;

        _groups = new HashSet<SubjectCourseGroup>();
        _assignments = new HashSet<Assignment>();
        _associations = new HashSet<SubjectCourseAssociation>();
        _mentors = new HashSet<Mentor>();
    }

    public virtual Subject Subject { get; protected init; }
    public string Name { get; protected init; }
    public virtual IReadOnlyCollection<SubjectCourseGroup> Groups => _groups;
    public virtual IReadOnlyCollection<Assignment> Assignments => _assignments;
    public virtual IReadOnlyCollection<SubjectCourseAssociation> Associations => _associations;
    public virtual IReadOnlyCollection<Mentor> Mentors => _mentors;

    public override string ToString() => Name;

    public SubjectCourseGroup AddGroup(StudentGroup group)
    {
        ArgumentNullException.ThrowIfNull(group);

        if (_groups.Any(x => x.StudentGroup.Equals(group)))
            throw new DomainInvalidOperationException($"Group {group} is already assigned to this course");

        var filters = new SubmissionQueueFilter[]
        {
            new GroupQueueFilter(new[] { group }),
            new SubmissionStateFilter(SubmissionState.Active),
            new SubjectCourseFilter(new[] { this })
        };

        var evaluators = new SubmissionEvaluator[]
        {
            new AssignmentDeadlineStateEvaluator(0, SortingOrder.Descending),
            new SubmissionDateTimeEvaluator(1, SortingOrder.Ascending),
        };

        var queue = new SubmissionQueue(filters, evaluators);
        var subjectCourseGroup = new SubjectCourseGroup(this, group, queue);
        
        _groups.Add(subjectCourseGroup);
        return subjectCourseGroup;
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

        var associationType = association.GetType();

        if (_associations.Any(a => a.GetType() == associationType))
            throw new DomainInvalidOperationException($"Course {this} already has {associationType} association");

        _associations.Add(association);
    }
    
    public void RemoveAssociation(SubjectCourseAssociation association)
    {
        ArgumentNullException.ThrowIfNull(association);

        if (!_associations.Remove(association))
            throw new DomainInvalidOperationException($"Association {association} is not assigned to this course");
    }

    public Mentor AddMentor(User user)
    {
        ArgumentNullException.ThrowIfNull(user);
        
        if (_mentors.Any(x => x.User.Equals(user)))
            throw new DomainInvalidOperationException($"User {user} is already a mentor of this subject course");
        
        var mentor = new Mentor(user);
        _mentors.Add(mentor);

        return mentor;
    }
    
    public void RemoveMentor(Mentor mentor)
    {
        ArgumentNullException.ThrowIfNull(mentor);
        
        if (!_mentors.Remove(mentor))
            throw new DomainInvalidOperationException($"Mentor {mentor} is not a mentor of this subject course");
    }
}