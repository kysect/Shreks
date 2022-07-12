namespace Kysect.Shreks.Core.DeadlineSpans;

public partial class AbsoluteDeadlineSpan : DeadlineSpan
{
    public AbsoluteDeadlineSpan(TimeSpan spanAfterDeadline, double absoluteValue) : base(spanAfterDeadline)
    {
        AbsoluteValue = absoluteValue;
    }

    public double AbsoluteValue { get; set; }

    public override double ProcessPoints(double points)
        => points - AbsoluteValue;
}