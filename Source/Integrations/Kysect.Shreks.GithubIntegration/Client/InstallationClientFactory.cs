using Octokit;

namespace Kysect.Shreks.GithubIntegration.Client;

public class InstallationClientFactory : IInstallationClientFactory
{
    private readonly IGitHubClient _gitHubAppClient;

    public InstallationClientFactory(IGitHubClient gitHubAppClient)
    {
        _gitHubAppClient = gitHubAppClient;
    }

    public async Task<GitHubClient> GetClient(long installationId)
    {
        var accessToken = await _gitHubAppClient.GitHubApps.CreateInstallationToken(installationId);

        return new GitHubClient(new ProductHeaderValue($"Installation-{installationId}"))
        {
            Credentials = new Credentials(accessToken.Token)
        };
        
    }
}