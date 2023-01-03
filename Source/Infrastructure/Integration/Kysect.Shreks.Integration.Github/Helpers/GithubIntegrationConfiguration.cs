namespace Kysect.Shreks.Integration.Github.Helpers;

public class GithubIntegrationConfiguration : IShreksConfiguration
{
    public GithubAuthConfiguration GithubAuthConfiguration { get; set; } = null!;

    public GithubAppConfiguration GithubAppConfiguration { get; set; } = null!;

    public void Verify()
    {
        GithubAuthConfiguration.Verify();
        GithubAppConfiguration.Verify();
    }
}