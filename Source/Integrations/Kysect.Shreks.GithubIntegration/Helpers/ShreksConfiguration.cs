namespace Kysect.Shreks.GithubIntegration.Helpers;

public sealed class ShreksConfiguration : IShreksConfiguration
{
    public CacheConfiguration CacheConfiguration { get; init; }
    public CacheEntryConfiguration CacheEntryConfiguration { get; init; }
    public GithubConfiguration GithubConfiguration { get; init; }

    public void Verify()
    {
        CacheConfiguration.Verify();
        CacheEntryConfiguration.Verify();
        GithubConfiguration.Verify();
    }
}