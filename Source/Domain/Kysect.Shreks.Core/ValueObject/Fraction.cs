using Kysect.Shreks.Common.Exceptions;

namespace Kysect.Shreks.Core.ValueObject;

public readonly record struct Fraction
{
    private const int Normalizer = 100;

    public Fraction(double value)
    {
        if (value is < 0 or > 1)
            throw new UnsupportedOperationException("Value of fraction must be between 0 and 1 (inclusive)");

        Value = value;
    }

    public static Fraction None => new Fraction(0);

    public double Value { get; }

    public static implicit operator Fraction(double value)
    {
        return new Fraction(value);
    }

    public static implicit operator double(Fraction fraction)
    {
        return fraction.Value;
    }

    public static Fraction FromDenormalizedValue(double value)
    {
        return new Fraction(value / Normalizer);
    }
}