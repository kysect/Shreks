using Kysect.Shreks.Core.ValueObject;

namespace Kysect.Shreks.Core.DeadlineSpans;

public class CappingDeadlineSpan : DeadlineSpan
{
    public CappingDeadlineSpan(TimeSpan spanAfterDeadline, double cap) : base(spanAfterDeadline)
    {
        Cap = cap;
    }

    public double Cap { get; set; }

    public override Rating ProcessRating(Rating points)
        => Math.Max(points, Cap);

    public override bool Equals(DeadlineSpan? other)
    {
        return other is CappingDeadlineSpan cappingDeadlineSpan &&
               cappingDeadlineSpan.Cap.Equals(Cap) &&
               base.Equals(cappingDeadlineSpan);
    }
}