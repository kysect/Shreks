namespace Kysect.Shreks.Core.DeadlineSpans;

public class FractionDeadlineSpan : DeadlineSpan
{
    public FractionDeadlineSpan(TimeSpan spanAfterDeadline, double fraction) : base(spanAfterDeadline)
    {
        Fraction = fraction;
    }

    public double Fraction { get; set; }

    public override double ProcessPoints(double points)
        => points * Fraction;

    public override bool Equals(DeadlineSpan? other)
    {
        return other is FractionDeadlineSpan fractionDeadlineSpan &&
               fractionDeadlineSpan.Fraction.Equals(Fraction) &&
               base.Equals(fractionDeadlineSpan);
    }
}