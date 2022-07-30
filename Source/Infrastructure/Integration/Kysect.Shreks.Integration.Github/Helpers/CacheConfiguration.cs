namespace Kysect.Shreks.Integration.Github.Helpers;

public class CacheConfiguration : IShreksConfiguration
{
    public int SizeLimit { get; init; }
    public TimeSpan Expiration { get; init; }

    public void Verify()
    {
        if (Expiration.TotalSeconds <= 0)
            throw new ArgumentException($"Expiration in {nameof(CacheConfiguration)} must be greater than 0");

        if (SizeLimit <= 0)
            throw new ArgumentException($"SizeLimit in {nameof(CacheConfiguration)} must be greater than 0");
    }
}