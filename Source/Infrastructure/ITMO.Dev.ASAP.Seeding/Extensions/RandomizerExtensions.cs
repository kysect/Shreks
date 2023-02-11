using Bogus;
using ITMO.Dev.ASAP.Core.ValueObject;

namespace ITMO.Dev.ASAP.Seeding.Extensions;

public static class RandomizerExtensions
{
    public static Points Points(this Randomizer randomizer, double min = 0.0d, double max = 1.0d)
    {
        return new Points(randomizer.Double(min, max));
    }

    public static Fraction Fraction(this Randomizer randomizer)
    {
        return new Fraction(randomizer.Double());
    }
}