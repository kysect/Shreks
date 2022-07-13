using Kysect.Shreks.Core.ValueObject;

namespace Kysect.Shreks.Core.DeadlinePolicies;

public class AbsoluteDeadlinePolicy : DeadlinePolicy
{
    public AbsoluteDeadlinePolicy(TimeSpan spanBeforeActivation, Rating absoluteValue) : base(spanBeforeActivation)
    {
        AbsoluteValue = absoluteValue;
    }

    public Rating AbsoluteValue { get; set; }

    public override Rating ProcessRating(Rating points)
        => points - AbsoluteValue;

    public override bool Equals(DeadlinePolicy? other)
    {
        return other is AbsoluteDeadlinePolicy absoluteDeadlineSpan &&
               absoluteDeadlineSpan.AbsoluteValue.Equals(AbsoluteValue) &&
               base.Equals(absoluteDeadlineSpan);
    }
}