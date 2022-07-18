using Octokit;

namespace Kysect.Shreks.GithubIntegration.client;

public interface IInstallationClientFactory
{
    GitHubClient GetClient(long installationId);
}