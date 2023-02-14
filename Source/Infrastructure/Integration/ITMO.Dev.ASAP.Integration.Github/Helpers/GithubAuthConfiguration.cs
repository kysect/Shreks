namespace ITMO.Dev.ASAP.Integration.Github.Helpers;

public class GithubAuthConfiguration : IAsapConfiguration
{
    public string? OAuthClientId { get; set; }

    public string? OAuthClientSecret { get; set; }

    public void Verify()
    {
        ArgumentNullException.ThrowIfNull(OAuthClientId);
        ArgumentNullException.ThrowIfNull(OAuthClientSecret);
    }
}