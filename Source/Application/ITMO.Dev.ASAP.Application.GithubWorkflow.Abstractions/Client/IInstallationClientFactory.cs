using Octokit;

namespace ITMO.Dev.ASAP.Application.GithubWorkflow.Abstractions.Client;

public interface IInstallationClientFactory
{
    GitHubClient GetClient(long installationId);
}