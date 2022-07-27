namespace Kysect.Shreks.Integration.Github.Helpers;

public class CacheConfiguration : IShreksConfiguration
{
    public int SizeLimit { get; init; }
    public int ExpirationMinutes { get; init; }

    public void Verify()
    {
        if (ExpirationMinutes <= 0)
            throw new ArgumentException($"ExpirationMinutes in {nameof(CacheConfiguration)} must be greater than 0");

        if (SizeLimit <= 0)
            throw new ArgumentException($"SizeLimit in {nameof(CacheConfiguration)} must be greater than 0");
    }
}