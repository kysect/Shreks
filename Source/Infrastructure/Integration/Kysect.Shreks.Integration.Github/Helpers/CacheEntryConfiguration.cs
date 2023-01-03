namespace Kysect.Shreks.Integration.Github.Helpers;

public class CacheEntryConfiguration : IShreksConfiguration
{
    public int EntrySize { get; init; }

    public TimeSpan SlidingExpiration { get; init; }

    public TimeSpan AbsoluteExpiration { get; init; }

    public void Verify()
    {
        if (EntrySize <= 0)
            throw new ArgumentException($"EntrySize in {nameof(CacheEntryConfiguration)} must be greater than 0");

        if (AbsoluteExpiration.TotalSeconds <= 0)
        {
            const string message = $"AbsoluteExpiration in {nameof(CacheEntryConfiguration)} must be greater than 0";
            throw new ArgumentException(message);
        }

        if (SlidingExpiration.TotalSeconds <= 0)
        {
            const string message = $"SlidingExpiration in {nameof(CacheEntryConfiguration)} must be greater than 0";
            throw new ArgumentException(message);
        }
    }
}