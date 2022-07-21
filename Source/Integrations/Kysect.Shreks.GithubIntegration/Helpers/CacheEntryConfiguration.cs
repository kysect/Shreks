namespace Kysect.Shreks.GithubIntegration.Helpers;

public class CacheEntryConfiguration
{
    public int EntrySize { get; set; }
    public int SlidingExpirationMinutes { get; set; }
    public int AbsoluteExpirationMinutes { get; set; }
} 