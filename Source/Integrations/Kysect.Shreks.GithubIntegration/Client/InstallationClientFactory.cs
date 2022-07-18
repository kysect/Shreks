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
        return new GitHubClient(new ProductHeaderValue($"Installation-{installationId}"),
            new InstallationCredentialStore(_gitHubAppClient, installationId));
    }
}