using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Core.ValueObject;
using RichEntity.Annotations;

namespace ITMO.Dev.ASAP.Core.Study;

public partial class Assignment : IEntity<Guid>
{
    private readonly HashSet<GroupAssignment> _groupAssignments;

    public Assignment(
        Guid id,
        string title,
        string shortName,
        int order,
        Points minPoints,
        Points maxPoints,
        SubjectCourse subjectCourse)
        : this(id)
    {
        ArgumentNullException.ThrowIfNull(title);

        if (minPoints > maxPoints)
            throw new ArgumentException("minPoints must be less than or equal to maxPoints");

        Title = title;
        ShortName = shortName;
        Order = order;
        MinPoints = minPoints;
        MaxPoints = maxPoints;
        SubjectCourse = subjectCourse;
        _groupAssignments = new HashSet<GroupAssignment>();

        subjectCourse.AddAssignment(this);
    }

    public string Title { get; set; }

    public string ShortName { get; set; }

    public int Order { get; set; }

    public Points MinPoints { get; protected set; }

    public Points MaxPoints { get; protected set; }

    public virtual SubjectCourse SubjectCourse { get; protected init; }

    public virtual IReadOnlyCollection<GroupAssignment> GroupAssignments => _groupAssignments;

    public void UpdateMinPoints(Points value)
    {
        if (value > MaxPoints)
        {
            string message = $"New minimal points ({value}) are greater than maximal points ({MaxPoints})";
            throw new DomainInvalidOperationException(message);
        }

        MinPoints = value;
    }

    public void UpdateMaxPoints(Points value)
    {
        if (value < MinPoints)
        {
            string message = $"New maximal points ({value}) are less than minimal points ({MinPoints})";
            throw new DomainInvalidOperationException(message);
        }

        MaxPoints = value;
    }

    public GroupAssignment AddGroup(StudentGroup group, DateOnly deadline)
    {
        var assignment = new GroupAssignment(group, this, deadline);

        if (!_groupAssignments.Add(assignment))
            throw new DomainInvalidOperationException($"Assignment {assignment} already exists");

        return assignment;
    }

    public void RemoveGroupAssignment(GroupAssignment assignment)
    {
        ArgumentNullException.ThrowIfNull(assignment);

        if (!_groupAssignments.Remove(assignment))
            throw new DomainInvalidOperationException($"Assignment {assignment} cannot be removed");
    }

    public override string ToString()
    {
        return $"Id: {Id}, Title: {ShortName}";
    }
}