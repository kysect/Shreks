namespace Kysect.Shreks.Core.Study.ValueObject;

public readonly record struct Rating(double Value)
{
    public static Rating None { get; } = new Rating(-1);

    public bool IsNone()
        => Value < 0;
}