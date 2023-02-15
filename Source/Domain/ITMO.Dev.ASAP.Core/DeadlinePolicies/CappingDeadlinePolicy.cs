using ITMO.Dev.ASAP.Core.ValueObject;

namespace ITMO.Dev.ASAP.Core.DeadlinePolicies;

public class CappingDeadlinePolicy : DeadlinePolicy
{
    public CappingDeadlinePolicy(TimeSpan spanBeforeActivation, double cap)
        : base(spanBeforeActivation)
    {
        Cap = new Points(cap);
    }

    protected CappingDeadlinePolicy() { }

    public Points Cap { get; set; }

    public override Points Apply(Points points)
    {
        return Points.Min(points, Cap);
    }

    public override bool Equals(DeadlinePolicy? other)
    {
        return other is CappingDeadlinePolicy cappingDeadlineSpan &&
               cappingDeadlineSpan.Cap.Equals(Cap) &&
               base.Equals(cappingDeadlineSpan);
    }
}