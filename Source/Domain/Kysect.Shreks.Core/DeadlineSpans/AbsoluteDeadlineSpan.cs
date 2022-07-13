namespace Kysect.Shreks.Core.DeadlineSpans;

public class AbsoluteDeadlineSpan : DeadlineSpan
{
    public AbsoluteDeadlineSpan(TimeSpan spanAfterDeadline, double absoluteValue) : base(spanAfterDeadline)
    {
        AbsoluteValue = absoluteValue;
    }

    public double AbsoluteValue { get; set; }

    public override double ProcessPoints(double points)
        => points - AbsoluteValue;

    public override bool Equals(DeadlineSpan? other)
    {
        return other is AbsoluteDeadlineSpan absoluteDeadlineSpan &&
               absoluteDeadlineSpan.AbsoluteValue.Equals(AbsoluteValue) &&
               base.Equals(absoluteDeadlineSpan);
    }
}