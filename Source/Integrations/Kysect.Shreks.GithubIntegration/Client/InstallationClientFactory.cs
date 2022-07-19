using Kysect.Shreks.GithubIntegration.CredentialStores;
using Octokit;

namespace Kysect.Shreks.GithubIntegration.client;

public class InstallationClientFactory : IInstallationClientFactory
{
    private readonly IGitHubClient _gitHubAppClient;

    public InstallationClientFactory(IGitHubClient gitHubAppClient)
    {
        _gitHubAppClient = gitHubAppClient;
    }

    public GitHubClient GetClient(long installationId)
    {
        return new GitHubClient(new ProductHeaderValue($"Installation-{installationId}"))
        {
            Credentials = new Credentials(_gitHubAppClient.GitHubApps.CreateInstallationToken(installationId).Result.Token)
        };
        
    }
}