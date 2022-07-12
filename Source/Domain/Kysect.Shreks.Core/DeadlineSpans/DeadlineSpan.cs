using RichEntity.Annotations;

namespace Kysect.Shreks.Core.DeadlineSpans;

public abstract partial class DeadlineSpan : IEntity<Guid>
{
    protected DeadlineSpan(TimeSpan spanAfterDeadline) : this(Guid.NewGuid())
    {
        SpanAfterDeadline = spanAfterDeadline;
    }

    public TimeSpan SpanAfterDeadline { get; protected init; }
    
    public abstract double ProcessPoints(double points);
}