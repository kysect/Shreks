using Kysect.Shreks.Core.ValueObject;

namespace Kysect.Shreks.Core.DeadlinePolicies;

public abstract class DeadlinePolicy : IEquatable<DeadlinePolicy>
{
    protected DeadlinePolicy(TimeSpan spanBeforeActivation)
    {
        SpanBeforeActivation = spanBeforeActivation;
    }

    protected DeadlinePolicy() { }

    public TimeSpan SpanBeforeActivation { get; protected init; }

    public virtual bool Equals(DeadlinePolicy? other)
    {
        return other?.SpanBeforeActivation.Equals(SpanBeforeActivation) ?? false;
    }

    public abstract Points Apply(Points points);

    public override bool Equals(object? obj)
    {
        return Equals(obj as DeadlinePolicy);
    }

    public override int GetHashCode()
    {
        return SpanBeforeActivation.GetHashCode();
    }

    public override string ToString()
    {
        return $"{GetType()} with span {SpanBeforeActivation}";
    }
}