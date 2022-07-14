using Kysect.Shreks.Core.ValueObject;

namespace Kysect.Shreks.Core.DeadlinePolicies;

public class CappingDeadlinePolicy : DeadlinePolicy
{
    public CappingDeadlinePolicy(TimeSpan spanBeforeActivation, double cap) : base(spanBeforeActivation)
    {
        Cap = cap;
    }

    public double Cap { get; set; }

    public override Points ProcessPoints(Points points)
        => Math.Max(points, Cap);

    public override bool Equals(DeadlinePolicy? other)
    {
        return other is CappingDeadlinePolicy cappingDeadlineSpan &&
               cappingDeadlineSpan.Cap.Equals(Cap) &&
               base.Equals(cappingDeadlineSpan);
    }
}