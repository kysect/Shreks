using Octokit;

namespace ITMO.Dev.ASAP.Application.GithubWorkflow.Abstractions.Client;

public interface IServiceOrganizationGithubClientProvider
{
    Task<GitHubClient> GetClient();
}