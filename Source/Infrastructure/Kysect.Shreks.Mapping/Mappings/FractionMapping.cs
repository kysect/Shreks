using Kysect.Shreks.Core.ValueObject;

namespace Kysect.Shreks.Mapping.Mappings;

public static class FractionMapping
{
    public static double AsDto(this Fraction fraction)
        => fraction.Value;

    public static Fraction AsFraction(this double fraction)
        => new Fraction(fraction);

    public static double? AsDto(this Fraction? fraction)
        => fraction?.Value;

    public static Fraction? AsFraction(this double? fraction)
        => fraction == null ? null : new Fraction(fraction.Value);
}