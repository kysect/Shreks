using Bogus.DataSets;

namespace Kysect.Shreks.Seeding.Extensions;

internal static class NameExtensions
{
    public static string MiddleName(this Name name)
    {
        return name.Random.Bool(0.95f)
            ? name.FirstName()
            : string.Empty;
    }
}