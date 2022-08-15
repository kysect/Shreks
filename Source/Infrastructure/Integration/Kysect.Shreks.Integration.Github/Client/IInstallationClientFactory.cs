using Octokit;

namespace Kysect.Shreks.Integration.Github.Client;

public interface IInstallationClientFactory
{
    GitHubClient GetClient(long installationId);
}