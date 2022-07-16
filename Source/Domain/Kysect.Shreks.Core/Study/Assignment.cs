using Kysect.Shreks.Core.DeadlinePolicies;
using Kysect.Shreks.Core.Exceptions;
using Kysect.Shreks.Core.ValueObject;
using RichEntity.Annotations;

namespace Kysect.Shreks.Core.Study;

public partial class Assignment : IEntity<Guid>
{
    private readonly HashSet<GroupAssignment> _assignments;
    private readonly HashSet<DeadlinePolicy> _deadlinePolicies;

    public Assignment(string title, Points minPoints, Points maxPoints)
        : this(Guid.NewGuid())
    {
        ArgumentNullException.ThrowIfNull(title);

        if (minPoints > maxPoints)
            throw new ArgumentException("minPoints must be less than or equal to maxPoints");
        
        Title = title;
        MinPoints = minPoints;
        MaxPoints = maxPoints;
        _assignments = new HashSet<GroupAssignment>();
        _deadlinePolicies = new HashSet<DeadlinePolicy>();
    }

    public string Title { get; set; }
    public Points MinPoints { get; protected set; }
    public Points MaxPoints { get; protected set; }
    public IReadOnlyCollection<GroupAssignment> GroupAssignments => _assignments;
    public IReadOnlyCollection<DeadlinePolicy> DeadlinePolicies => _deadlinePolicies;

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
        
        if (!_assignments.Add(assignment))
            throw new DomainInvalidOperationException($"Assignment {assignment} already exists");
    }
    
    public void RemoveGroupAssignment(GroupAssignment assignment)
    {
        ArgumentNullException.ThrowIfNull(assignment);
        
        if (!_assignments.Remove(assignment))
            throw new DomainInvalidOperationException($"Assignment {assignment} cannot be removed");
    }
    
    public void AddDeadlinePolicy(DeadlinePolicy policy)
    {
        ArgumentNullException.ThrowIfNull(policy);

        if (!_deadlinePolicies.Add(policy))
            throw new DomainInvalidOperationException($"Deadline span {policy} already exists");
    }
    
    public void RemoveDeadlinePolicy(DeadlinePolicy policy)
    {
        ArgumentNullException.ThrowIfNull(policy);
        
        if (!_deadlinePolicies.Remove(policy))
            throw new DomainInvalidOperationException($"Deadline span {policy} cannot be removed");
    }
}