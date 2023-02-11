using Octokit;

namespace ITMO.Dev.ASAP.Application.GithubWorkflow.Abstractions.Client;

public interface IOrganizationGithubClientProvider
{
    Task<GitHubClient> GetClient(string organization);
}