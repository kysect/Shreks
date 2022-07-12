using Ardalis.Result;
using Kysect.Shreks.Core.DeadlineSpans;
using RichEntity.Annotations;

namespace Kysect.Shreks.Core.Study;

public partial class Assignment : IEntity<Guid>
{
    private readonly List<GroupAssignment> _assignments;
    private readonly List<DeadlineSpan> _deadlineSpans;

    public Assignment(string title, double minPoints, double maxPoints)
        : this(Guid.NewGuid())
    {
        ArgumentNullException.ThrowIfNull(title);

        if (minPoints > maxPoints)
            throw new ArgumentException("minPoints must be less than or equal to maxPoints");
        
        Title = title;
        MinPoints = minPoints;
        MaxPoints = maxPoints;
        _assignments = new List<GroupAssignment>();
        _deadlineSpans = new List<DeadlineSpan>();
    }

    public string Title { get; set; }
    public double MinPoints { get; protected set; }
    public double MaxPoints { get; protected set; }
    public IReadOnlyCollection<GroupAssignment> GroupAssignments => _assignments.AsReadOnly();
    public IReadOnlyCollection<DeadlineSpan> DeadlineSpans => _deadlineSpans.AsReadOnly();

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
    
    public Result AddDeadlineSpan(DeadlineSpan span)
    {
        if (_deadlineSpans.Contains(span))
            return Result.Error($"Deadline span {span} already exists");
        
        _deadlineSpans.Add(span);
        return Result.Success();
    }
    
    public Result RemoveDeadlineSpan(DeadlineSpan span)
    {
        if (!_deadlineSpans.Contains(span))
            return Result.Error($"Deadline span {span} does not exist");
        
        _deadlineSpans.Remove(span);
        return Result.Success();
    }
}