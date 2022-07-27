namespace Kysect.Shreks.GoogleIntegration.Tools;

public static class RandomUtilities
{
    private static readonly Random Random = new();

    public static int GetRandomId() => Random.Next();
}