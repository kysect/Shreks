namespace Kysect.Shreks.GithubIntegration.Helpers;

public class CacheEntryConfiguration : IShreksConfiguration
{
    public int EntrySize { get; init; }
    public int SlidingExpirationMinutes { get; init; }
    public int AbsoluteExpirationMinutes { get; init; }

    public void Verify()
    {
        if (EntrySize <= 0)
            throw new ArgumentException($"EntrySize in {nameof(CacheEntryConfiguration)} must be greater than 0");
        
        if (AbsoluteExpirationMinutes <= 0)
            throw new ArgumentException($"AbsoluteExpirationMinutes in {nameof(CacheEntryConfiguration)} must be greater than 0");

        if (SlidingExpirationMinutes <= 0)
            throw new ArgumentException($"SlidingExpirationMinutes in {nameof(CacheEntryConfiguration)} must be greater than 0");
    }
} 