using System.Globalization;

namespace Kysect.Shreks.Core.ValueObject;

public readonly record struct Points : IComparable<Points>
{
    private const int MaxSignCount = 2;

    public Points(double value)
    {
        Value = value >= 0 ? Math.Round(value, MaxSignCount) : 0;
    }

    public double Value { get; }

    public static Points None => new Points();

    public int CompareTo(Points other)
    {
        return Value.CompareTo(other.Value);
    }

    public static Points Min(Points a, Points b)
    {
        return a < b ? a : b;
    }

    public static Points Max(Points a, Points b)
    {
        return a > b ? a : b;
    }

    public static implicit operator Points(double value)
    {
        return new Points(value);
    }

    public static Points operator +(Points a, Points b)
    {
        return new Points(a.Value + b.Value);
    }

    public static Points operator -(Points a, Points b)
    {
        return new Points(a.Value - b.Value);
    }

    public static Points operator *(Points a, Fraction b)
    {
        return new Points(a.Value * b.Value);
    }

    public static bool operator >(Points a, Points b)
    {
        return a.Value > b.Value;
    }

    public static bool operator <(Points a, Points b)
    {
        return a.Value < b.Value;
    }

    public override string ToString()
    {
        return Value.ToString(CultureInfo.InvariantCulture);
    }
}