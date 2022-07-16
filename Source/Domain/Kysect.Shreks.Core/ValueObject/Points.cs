namespace Kysect.Shreks.Core.ValueObject;

public readonly record struct Points
{
    public Points(double value)
    {
        Value = value >= 0 ? value : 0;
    }

    public double Value { get; }

    public static Points None => new Points();

    public static implicit operator Points(double value)
        => new Points(value);

    public static implicit operator double(Points points)
        => points.Value;

    public static Points operator +(Points a, Points b)
        => new Points(a.Value + b.Value);

    public static Points operator -(Points a, Points b)
        => new Points(a.Value - b.Value);

    public static Points operator *(Points a, Points b)
        => new Points(a.Value * b.Value);

    public static Points operator /(Points a, Points b)
    {
        if (b.Value is 0)
            throw new DivideByZeroException();
        
        return new Points(a.Value / b.Value);
    }
}