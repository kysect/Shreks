using Octokit;

namespace Kysect.Shreks.GithubIntegration.Client;

public interface IInstallationClientFactory
{
    Task<GitHubClient> GetClient(long installationId);
}