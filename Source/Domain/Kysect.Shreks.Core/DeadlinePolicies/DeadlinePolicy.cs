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

    public abstract Points Apply(Points points);

    public virtual bool Equals(DeadlinePolicy? other)
        => other?.SpanBeforeActivation.Equals(SpanBeforeActivation) ?? false;

    public override bool Equals(object? obj)
        => Equals(obj as DeadlinePolicy);

    public override int GetHashCode()
        => SpanBeforeActivation.GetHashCode();


    public override string ToString()
    {
        return $"{GetType()} with span {SpanBeforeActivation}";
    }
}