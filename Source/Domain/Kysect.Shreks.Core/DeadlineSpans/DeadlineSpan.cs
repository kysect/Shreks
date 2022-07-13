using Kysect.Shreks.Core.ValueObject;

namespace Kysect.Shreks.Core.DeadlineSpans;

public abstract class DeadlineSpan : IEquatable<DeadlineSpan>
{
    protected DeadlineSpan(TimeSpan spanAfterDeadline)
    {
        SpanAfterDeadline = spanAfterDeadline;
    }

    public TimeSpan SpanAfterDeadline { get; protected init; }
    
    public abstract Rating ProcessRating(Rating points);

    public virtual bool Equals(DeadlineSpan? other)
    {
        return other?.SpanAfterDeadline.Equals(SpanAfterDeadline) ?? false;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as DeadlineSpan);
    }

    public override int GetHashCode()
        => SpanAfterDeadline.GetHashCode();
}