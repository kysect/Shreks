using Kysect.Shreks.Integration.Github.CredentialStores;
using Kysect.Shreks.Integration.Github.Entities;
using Octokit;

namespace Kysect.Shreks.Integration.Github.Client;

public class InstallationClientFactory : IInstallationClientFactory
{
    private readonly IShreksMemoryCache _memoryCache;
    private readonly IGitHubClient _gitHubAppClient;

    public InstallationClientFactory(IGitHubClient gitHubAppClient, IShreksMemoryCache memoryCache)
    {
        _gitHubAppClient = gitHubAppClient;
        _memoryCache = memoryCache;
    }

    public async Task<GitHubClient> GetClient(long installationId)
    {
        return await _memoryCache.GetOrCreateAsync(installationId, async _ => await CreateInstallationClient(installationId));
    }

    private async Task<GitHubClient> CreateInstallationClient(long installationId)
    {
        return new GitHubClient(new ProductHeaderValue($"Installation-{installationId}"), 
            new InstallationCredentialStore(_gitHubAppClient, installationId));
    }
}
