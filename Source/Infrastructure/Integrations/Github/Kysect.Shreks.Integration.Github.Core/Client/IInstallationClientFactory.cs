using Octokit;

namespace Kysect.Shreks.Integration.Github.Core.Client;

public interface IInstallationClientFactory
{
    Task<GitHubClient> GetClient(long installationId);
}