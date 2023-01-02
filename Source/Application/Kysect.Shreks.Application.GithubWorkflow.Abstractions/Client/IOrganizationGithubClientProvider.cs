using Octokit;

namespace Kysect.Shreks.Application.GithubWorkflow.Abstractions.Client;

public interface IOrganizationGithubClientProvider
{
    Task<GitHubClient> GetClient(string organization);
}