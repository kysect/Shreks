namespace Kysect.Shreks.Core.DeadlineSpans;

public partial class CappingDeadlineSpan : DeadlineSpan
{
    public CappingDeadlineSpan(TimeSpan spanAfterDeadline, double cap) : base(spanAfterDeadline)
    {
        Cap = cap;
    }

    public double Cap { get; set; }

    public override double ProcessPoints(double points)
        => Math.Max(points, Cap);
}