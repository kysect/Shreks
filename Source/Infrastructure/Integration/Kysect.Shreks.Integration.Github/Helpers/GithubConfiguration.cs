namespace Kysect.Shreks.Integration.Github.Helpers;

public class GithubConfiguration : IShreksConfiguration
{
    public string? PrivateKeySource { get; init; }
    public int AppIntegrationId { get; init; }
    public int ExpirationSeconds { get; init; }
    public string? Organization { get; init; }
    public string? GithubAppSecret { get; set; }
    public string? OAuthClientId { get; set; }
    public string? OAuthClientSecret { get; set; }

    public void SetGithubAppSecret(string githubAppSecret)
    {
        ArgumentNullException.ThrowIfNull(githubAppSecret, nameof(githubAppSecret));
        GithubAppSecret = githubAppSecret;
    }

    public void Verify()
    {
        ArgumentNullException.ThrowIfNull(Organization, nameof(Organization));
        ArgumentNullException.ThrowIfNull(GithubAppSecret, nameof(GithubAppSecret));
        ArgumentNullException.ThrowIfNull(PrivateKeySource, nameof(PrivateKeySource));
        ArgumentNullException.ThrowIfNull(OAuthClientId, nameof(OAuthClientId));
        ArgumentNullException.ThrowIfNull(OAuthClientSecret, nameof(OAuthClientSecret));

        if (ExpirationSeconds <= 0)
            throw new ArgumentException($"Expiration in {nameof(GithubConfiguration)} must be greater than 0");
        
        if (AppIntegrationId <= 0)
            throw new ArgumentException($"AppIntegrationId in {nameof(GithubConfiguration)} must be greater than 0");
    }
}
