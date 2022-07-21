namespace Kysect.Shreks.GithubIntegration.Helpers;

public class CacheEntryConfiguration
{
    public int EntrySize { get; init; }
    public int SlidingExpirationMinutes { get; init; }
    public int AbsoluteExpirationMinutes { get; init; }
} 