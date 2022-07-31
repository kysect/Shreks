namespace Kysect.Shreks.Integration.Github.Helpers;

public class GithubConfiguration : IShreksConfiguration
{
    public string? PrivateKeySource { get; init; }
    public string? Secret { get; init; }
    public int AppIntegrationId { get; init; }
    public int ExpirationSeconds { get; init; }
    public string? Organization { get; init; }

    public void Verify()
    {
        ArgumentNullException.ThrowIfNull(Organization, nameof(Organization));
        ArgumentNullException.ThrowIfNull(Secret, nameof(Secret));
        ArgumentNullException.ThrowIfNull(PrivateKeySource, nameof(PrivateKeySource));

        if (ExpirationSeconds <= 0)
            throw new ArgumentException($"Expiration in {nameof(GithubConfiguration)} must be greater than 0");
        
        if (AppIntegrationId <= 0)
            throw new ArgumentException($"AppIntegrationId in {nameof(GithubConfiguration)} must be greater than 0");
    }
}
