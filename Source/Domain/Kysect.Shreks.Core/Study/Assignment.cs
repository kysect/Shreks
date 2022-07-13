using Ardalis.Result;
using Kysect.Shreks.Core.DeadlineSpans;
using RichEntity.Annotations;

namespace Kysect.Shreks.Core.Study;

public partial class Assignment : IEntity<Guid>
{
    private readonly HashSet<GroupAssignment> _assignments;
    private readonly HashSet<DeadlinePolicy> _deadlinePolicies;

    public Assignment(string title, double minPoints, double maxPoints)
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
    public double MinPoints { get; protected set; }
    public double MaxPoints { get; protected set; }
    public IReadOnlyCollection<GroupAssignment> GroupAssignments => _assignments;
    public IReadOnlyCollection<DeadlinePolicy> DeadlinePolicies => _deadlinePolicies;

    public Result UpdateMinPoints(double value)
    {
        if (value > MaxPoints)
            return Result.Error($"New minimal points ({value}) are greater than maximal points ({MaxPoints})");
        
        MinPoints = value;
        return Result.Success();
    }
    
    public Result UpdateMaxPoints(double value)
    {
        if (value < MinPoints)
            return Result.Error($"New maximal points ({value}) are less than minimal points ({MinPoints})");
        
        MaxPoints = value;
        return Result.Success();
    }
    
    public Result AddDeadlinePolicy(DeadlinePolicy policy)
    {
        if (!_deadlinePolicies.Add(policy))
            return Result.Error($"Deadline span {policy} already exists");
        
        return Result.Success();
    }
    
    public Result RemoveDeadlinePolicy(DeadlinePolicy policy)
    {
        if (!_deadlinePolicies.Remove(policy))
            return Result.Error($"Deadline span {policy} cannot be removed");
        
        return Result.Success();
    }
}