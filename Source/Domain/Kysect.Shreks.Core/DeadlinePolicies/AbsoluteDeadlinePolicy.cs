using Kysect.Shreks.Core.ValueObject;

namespace Kysect.Shreks.Core.DeadlinePolicies;

public class AbsoluteDeadlinePolicy : DeadlinePolicy
{
    public AbsoluteDeadlinePolicy(TimeSpan spanBeforeActivation, Points absoluteValue) : base(spanBeforeActivation)
    {
        AbsoluteValue = absoluteValue;
    }

    public Points AbsoluteValue { get; set; }

    public override Points ProcessPoints(Points points)
        => points - AbsoluteValue;

    public override bool Equals(DeadlinePolicy? other)
    {
        return other is AbsoluteDeadlinePolicy absoluteDeadlineSpan &&
               absoluteDeadlineSpan.AbsoluteValue.Equals(AbsoluteValue) &&
               base.Equals(absoluteDeadlineSpan);
    }
}