namespace Kysect.Shreks.Core.ValueObject;

public readonly record struct Rating
{
    public Rating(double value)
    {
        Value = value >= 0 ? value : 0;
    }

    public double Value { get; }

    public static Rating None => new Rating();

    public static implicit operator Rating(double value)
        => new Rating(value);

    public static implicit operator double(Rating rating)
        => rating.Value;

    public static Rating operator +(Rating a, Rating b)
        => new Rating(a.Value + b.Value);

    public static Rating operator -(Rating a, Rating b)
        => new Rating(a.Value - b.Value);

    public static Rating operator *(Rating a, Rating b)
        => new Rating(a.Value * b.Value);

    public static Rating operator /(Rating a, Rating b)
    {
        if (b.Value is 0)
            throw new DivideByZeroException();
        
        return new Rating(a.Value / b.Value);
    }
}