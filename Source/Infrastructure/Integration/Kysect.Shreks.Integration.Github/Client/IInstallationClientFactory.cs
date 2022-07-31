using Octokit;

namespace Kysect.Shreks.Integration.Github.Client;

public interface IInstallationClientFactory
{
    Task<GitHubClient> GetClient(long installationId);
}