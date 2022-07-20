namespace Kysect.Shreks.GithubIntegration.Helpers;

public class GithubConfiguration
{
    public string? PrivateKeySource { get; init; }
    public string? Secret { get; init; }
    public int AppIntegrationId { get; init; }
    public int ExpirationSeconds { get; init; }
    public string? Organization { get; init; }
}
