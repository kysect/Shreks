namespace ITMO.Dev.ASAP.Integration.Github.Helpers;

public class GithubIntegrationConfiguration : IAsapConfiguration
{
    public GithubAuthConfiguration GithubAuthConfiguration { get; set; } = null!;

    public GithubAppConfiguration GithubAppConfiguration { get; set; } = null!;

    public void Verify()
    {
        GithubAuthConfiguration.Verify();
        GithubAppConfiguration.Verify();
    }
}