using Kysect.Shreks.Core.ValueObject;

namespace Kysect.Shreks.Core.DeadlinePolicies;

public class FractionDeadlinePolicy : DeadlinePolicy
{
    public FractionDeadlinePolicy(TimeSpan spanBeforeActivation, Fraction fraction) : base(spanBeforeActivation)
    {
        Fraction = fraction;
    }

    public Fraction Fraction { get; set; }

    public override Points Apply(Points points)
        => points * Fraction;

    public override bool Equals(DeadlinePolicy? other)
    {
        return other is FractionDeadlinePolicy fractionDeadlineSpan &&
               fractionDeadlineSpan.Fraction.Equals(Fraction) &&
               base.Equals(fractionDeadlineSpan);
    }
}