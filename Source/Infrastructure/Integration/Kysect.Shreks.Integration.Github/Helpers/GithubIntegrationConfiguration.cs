namespace Kysect.Shreks.Integration.Github.Helpers;

public class GithubIntegrationConfiguration : IShreksConfiguration
{
    public GithubAuthConfiguration GithubAuthConfiguration { get; set; }
    public GithubAppConfiguration GithubAppConfiguration { get; set; }

    public void Verify()
    {
        GithubAuthConfiguration.Verify();
        GithubAppConfiguration.Verify();
    }
}
