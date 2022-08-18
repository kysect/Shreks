using System.Globalization;

namespace Kysect.Shreks.Core.ValueObject;

public readonly record struct Rating
{
    private const double MaxRating = 100;

    public Rating(double value)
    {
        if (value is < 0 or > MaxRating)
            throw new ArgumentOutOfRangeException(nameof(value), "Value must be between 0 and 1");

        Value = value;
    }

    public double Value { get; }

    public static Rating None() => new Rating();

    public static implicit operator Rating(double value)
        => new Rating(value);

    public static implicit operator double(Rating rating)
        => rating.Value;

    public static double operator *(Rating rating, double value)
        => rating.Value / MaxRating * value;

    public static bool operator >(Rating a, Rating b)
        => a.Value > b.Value;

    public static bool operator <(Rating a, Rating b)
        => a.Value < b.Value;

    public override string ToString()
        => Value.ToString(CultureInfo.InvariantCulture);
}