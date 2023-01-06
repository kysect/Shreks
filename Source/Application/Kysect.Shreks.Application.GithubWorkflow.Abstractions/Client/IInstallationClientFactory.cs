using Octokit;

namespace Kysect.Shreks.Application.GithubWorkflow.Abstractions.Client;

public interface IInstallationClientFactory
{
    GitHubClient GetClient(long installationId);
}