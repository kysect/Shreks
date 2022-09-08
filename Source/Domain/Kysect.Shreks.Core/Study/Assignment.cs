using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.Queue.Filters;
using Kysect.Shreks.Core.ValueObject;
using RichEntity.Annotations;

namespace Kysect.Shreks.Core.Study;

public partial class Assignment : IEntity<Guid>
{
    private readonly HashSet<GroupAssignment> _groupAssignments;

    // TODO: Remove when .NET 7 is released
    protected virtual IReadOnlyCollection<SubmissionQueueFilter> Filters { get; init; }

    public Assignment(string title, string shortName, int order, Points minPoints, Points maxPoints, SubjectCourse subjectCourse)
        : this(Guid.NewGuid())
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
            var message = $"New minimal points ({value}) are greater than maximal points ({MaxPoints})";
            throw new DomainInvalidOperationException(message);
        }

        MinPoints = value;
    }

    public void UpdateMaxPoints(Points value)
    {
        if (value < MinPoints)
        {
            var message = $"New maximal points ({value}) are less than minimal points ({MinPoints})";
            throw new DomainInvalidOperationException(message);
        }

        MaxPoints = value;
    }

    public void AddGroupAssignment(GroupAssignment assignment)
    {
        ArgumentNullException.ThrowIfNull(assignment);

        if (!_groupAssignments.Add(assignment))
            throw new DomainInvalidOperationException($"Assignment {assignment} already exists");
    }

    public void RemoveGroupAssignment(GroupAssignment assignment)
    {
        ArgumentNullException.ThrowIfNull(assignment);

        if (!_groupAssignments.Remove(assignment))
            throw new DomainInvalidOperationException($"Assignment {assignment} cannot be removed");
    }

    public override String ToString()
    {
        return $"Id: {Id}, Title: {ShortName}";
    }
}