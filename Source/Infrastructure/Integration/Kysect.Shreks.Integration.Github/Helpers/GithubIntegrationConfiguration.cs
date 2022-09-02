namespace Kysect.Shreks.Integration.Github.Helpers;

public class GithubIntegrationConfiguration : IShreksConfiguration
{
    public GithubAuthConfiguration AuthConfiguration { get; set; }
    public GithubAppConfiguration GithubAppConfiguration { get; set; }

    public void Verify()
    {
        AuthConfiguration.Verify();
        GithubAppConfiguration.Verify();
    }
}
