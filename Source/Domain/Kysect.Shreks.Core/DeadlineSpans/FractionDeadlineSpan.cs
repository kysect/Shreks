using Kysect.Shreks.Core.ValueObject;

namespace Kysect.Shreks.Core.DeadlineSpans;

public class FractionDeadlineSpan : DeadlineSpan
{
    public FractionDeadlineSpan(TimeSpan spanAfterDeadline, Fraction fraction) : base(spanAfterDeadline)
    {
        Fraction = fraction;
    }

    public Fraction Fraction { get; set; }

    public override Rating ProcessRating(Rating points)
        => points * Fraction;

    public override bool Equals(DeadlineSpan? other)
    {
        return other is FractionDeadlineSpan fractionDeadlineSpan &&
               fractionDeadlineSpan.Fraction.Equals(Fraction) &&
               base.Equals(fractionDeadlineSpan);
    }
}