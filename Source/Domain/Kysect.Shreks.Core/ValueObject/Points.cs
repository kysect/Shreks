using System.Globalization;

namespace Kysect.Shreks.Core.ValueObject;

public readonly record struct Points
{
    public Points(double value)
    {
        Value = value >= 0 ? value : 0;
    }

    public double Value { get; }

    public static Points None => new Points();

    public static Points Max(Points a, Points b)
    {
        return a > b ? a : b;
    }

    public static Points operator +(Points a, Points b)
        => new Points(a.Value + b.Value);

    public static Points operator -(Points a, Points b)
        => new Points(a.Value - b.Value);

    public static Points operator *(Points a, Points b)
        => new Points(a.Value * b.Value);

    public static Points operator *(Points a, Rating b)
        => new Points(a.Value * b.Value);

    public static Points operator *(Points a, Fraction b)
        => new Points(a.Value * b.Value);

    public static Points operator /(Points a, Points b)
    {
        if (b.Value is 0)
            throw new DivideByZeroException();

        return new Points(a.Value / b.Value);
    }

    public static bool operator >(Points a, Points b)
        => a.Value > b.Value;

    public static bool operator <(Points a, Points b)
        => a.Value < b.Value;

    public override string ToString()
        => Value.ToString(CultureInfo.InvariantCulture);
}