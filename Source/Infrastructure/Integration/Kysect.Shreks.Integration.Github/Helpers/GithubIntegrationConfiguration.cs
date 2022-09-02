namespace Kysect.Shreks.Integration.Github.Helpers;

public class GithubIntegrationConfiguration : IShreksConfiguration
{
    public GithubAuthConfiguration AuthConfiguration { get; set; }
    public GithubAppConfiguration GithubAuthConfiguration { get; set; }

    public void Verify()
    {
        AuthConfiguration.Verify();
        GithubAuthConfiguration.Verify();
    }
}
