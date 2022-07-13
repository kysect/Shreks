using Kysect.Shreks.Core.ValueObject;

namespace Kysect.Shreks.Core.DeadlineSpans;

public class AbsoluteDeadlineSpan : DeadlineSpan
{
    public AbsoluteDeadlineSpan(TimeSpan spanAfterDeadline, Rating absoluteValue) : base(spanAfterDeadline)
    {
        AbsoluteValue = absoluteValue;
    }

    public Rating AbsoluteValue { get; set; }

    public override Rating ProcessRating(Rating points)
        => points - AbsoluteValue;

    public override bool Equals(DeadlineSpan? other)
    {
        return other is AbsoluteDeadlineSpan absoluteDeadlineSpan &&
               absoluteDeadlineSpan.AbsoluteValue.Equals(AbsoluteValue) &&
               base.Equals(absoluteDeadlineSpan);
    }
}