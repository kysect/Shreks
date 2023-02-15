using ITMO.Dev.ASAP.Core.ValueObject;

namespace ITMO.Dev.ASAP.Mapping.Mappings;

public static class FractionMapping
{
    public static double AsDto(this Fraction fraction)
    {
        return fraction.Value;
    }

    public static double? AsDto(this Fraction? fraction)
    {
        return fraction?.Value;
    }

    public static Fraction AsFraction(this double fraction)
    {
        return new Fraction(fraction);
    }

    public static Fraction? AsFraction(this double? fraction)
    {
        return fraction == null ? null : new Fraction(fraction.Value);
    }
}