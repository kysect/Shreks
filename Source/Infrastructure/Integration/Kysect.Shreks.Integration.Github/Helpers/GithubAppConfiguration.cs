namespace Kysect.Shreks.Integration.Github.Helpers;

public class GithubAppConfiguration : IShreksConfiguration
{
    public string? PrivateKeySource { get; init; }
    public int AppIntegrationId { get; init; }
    public int JwtExpirationSeconds { get; init; }
    public string? GithubAppSecret { get; set; }

    public void Verify()
    {
        ArgumentNullException.ThrowIfNull(GithubAppSecret, nameof(GithubAppSecret));
        ArgumentNullException.ThrowIfNull(PrivateKeySource, nameof(PrivateKeySource));


        if (JwtExpirationSeconds <= 0)
            throw new ArgumentException($"Expiration in {nameof(GithubIntegrationConfiguration)} must be greater than 0");

        if (AppIntegrationId <= 0)
            throw new ArgumentException($"AppIntegrationId in {nameof(GithubIntegrationConfiguration)} must be greater than 0");
    }
}