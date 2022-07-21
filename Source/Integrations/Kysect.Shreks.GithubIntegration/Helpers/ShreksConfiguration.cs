namespace Kysect.Shreks.GithubIntegration.Helpers;

public sealed class ShreksConfiguration
{
    public CacheConfiguration CacheConfiguration { get; init; }
    public CacheEntryConfiguration CacheEntryConfiguration { get; init; }
    public GithubConfiguration GithubConfiguration { get; init; }

    public void Verify()
    {
        VerifyGithubConfiguration(GithubConfiguration);
        VerifyCacheEntryConfiguration(CacheEntryConfiguration);
        VerifyCacheConfiguration(CacheConfiguration);
    }
    
    private void VerifyGithubConfiguration(GithubConfiguration githubConfiguration)
    {
        ArgumentNullException.ThrowIfNull(githubConfiguration, nameof(githubConfiguration));
        ArgumentNullException.ThrowIfNull(githubConfiguration.Organization, nameof(githubConfiguration.Organization));
        ArgumentNullException.ThrowIfNull(githubConfiguration.Secret, nameof(githubConfiguration.Secret));
        ArgumentNullException.ThrowIfNull(githubConfiguration.PrivateKeySource, nameof(githubConfiguration.PrivateKeySource));

        if (githubConfiguration.ExpirationSeconds <= 0)
            throw new ArgumentException($"ExpirationMinutes in {nameof(githubConfiguration)} must be greater than 0");
        
        if (githubConfiguration.AppIntegrationId <= 0)
            throw new ArgumentException($"AppIntegrationId in {nameof(githubConfiguration)} must be greater than 0");
    }

    private void VerifyCacheConfiguration(CacheConfiguration cacheConfiguration)
    {
        ArgumentNullException.ThrowIfNull(cacheConfiguration);

        if (cacheConfiguration.ExpirationMinutes <= 0)
            throw new ArgumentException($"ExpirationMinutes in {nameof(cacheConfiguration)} must be greater than 0");
        
        if (cacheConfiguration.SizeLimit <= 0)
            throw new ArgumentException($"SizeLimit in {nameof(cacheConfiguration)} must be greater than 0");
    }

    private void VerifyCacheEntryConfiguration(CacheEntryConfiguration cacheEntryConfiguration)
    {
        ArgumentNullException.ThrowIfNull(cacheEntryConfiguration);

        if (cacheEntryConfiguration.EntrySize <= 0)
            throw new ArgumentException($"EntrySize in {nameof(cacheEntryConfiguration)} must be greater than 0");
        
        if (cacheEntryConfiguration.AbsoluteExpirationMinutes <= 0)
            throw new ArgumentException($"AbsoluteExpirationMinutes in {nameof(cacheEntryConfiguration)} must be greater than 0");

        if (cacheEntryConfiguration.SlidingExpirationMinutes <= 0)
            throw new ArgumentException($"SlidingExpirationMinutes in {nameof(cacheEntryConfiguration)} must be greater than 0");
    }
}