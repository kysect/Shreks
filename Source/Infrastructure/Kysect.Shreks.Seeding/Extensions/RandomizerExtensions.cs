using Bogus;
using Kysect.Shreks.Core.ValueObject;

namespace Kysect.Shreks.Seeding.Extensions;

public static class RandomizerExtensions
{
    public static Points Points(this Randomizer randomizer, double min = 0.0d, double max = 1.0d)
    {
        return new Points(randomizer.Double(min, max));
    }

    public static Rating Rating(this Randomizer randomizer, double min = 0.0d, double max = 1.0d)
    {
        return new Rating(randomizer.Double(min, max));
    }
}