using Kysect.Shreks.GithubIntegration.Entities;
using Microsoft.Extensions.Caching.Memory;
using Octokit;

namespace Kysect.Shreks.GithubIntegration.Client;

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
        return await GetClientFromCache(installationId);
    }

    private async Task<GitHubClient> GetClientFromCache(long installationId)
    {
        if (_memoryCache.TryGetValue(installationId, out GitHubClient client)) return client;

        var accessToken = await _gitHubAppClient.GitHubApps.CreateInstallationToken(installationId);

        client = new GitHubClient(new ProductHeaderValue($"Installation-{installationId}"))
        {
            Credentials = new Credentials(accessToken.Token)
        };

        _memoryCache.Set(installationId, client, new MemoryCacheEntryOptions().SetSize(1));

        return client;
    }
}