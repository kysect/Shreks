namespace Kysect.Shreks.Integration.Github.Helpers;

public class GithubAuthConfiguration : IShreksConfiguration
{
    public string? OAuthClientId { get; set; }
    public string? OAuthClientSecret { get; set; }

    public void Verify()
    {
        ArgumentNullException.ThrowIfNull(OAuthClientId);
        ArgumentNullException.ThrowIfNull(OAuthClientSecret);
    }
}