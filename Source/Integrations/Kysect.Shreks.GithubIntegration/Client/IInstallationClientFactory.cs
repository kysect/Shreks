using Octokit;

namespace Kysect.Shreks.GithubIntegration.client;

public interface IInstallationClientFactory
{
    Task<GitHubClient> GetClient(long installationId);
}