using Kysect.Shreks.Core.ValueObject;

namespace Kysect.Shreks.Core.DeadlineSpans;

public abstract class DeadlinePolicy : IEquatable<DeadlinePolicy>
{
    protected DeadlinePolicy(TimeSpan spanBeforeActivation)
    {
        SpanBeforeActivation = spanBeforeActivation;
    }

    public TimeSpan SpanBeforeActivation { get; protected init; }
    
    public abstract Rating ProcessRating(Rating points);

    public virtual bool Equals(DeadlinePolicy? other)
        => other?.SpanBeforeActivation.Equals(SpanBeforeActivation) ?? false;

    public override bool Equals(object? obj)
        => Equals(obj as DeadlinePolicy);

    public override int GetHashCode()
        => SpanBeforeActivation.GetHashCode();
}