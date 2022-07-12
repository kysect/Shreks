namespace Kysect.Shreks.Core.DeadlineSpans;

public partial class FractionDeadlineSpan : DeadlineSpan
{
    public FractionDeadlineSpan(TimeSpan spanAfterDeadline, double fraction) : base(spanAfterDeadline)
    {
        Fraction = fraction;
    }

    public double Fraction { get; set; }

    public override double ProcessPoints(double points)
        => points * Fraction;
}