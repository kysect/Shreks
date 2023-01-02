using Octokit;

namespace Kysect.Shreks.Application.GithubWorkflow.Abstractions.Client;

public interface IServiceOrganizationGithubClientProvider
{
    Task<GitHubClient> GetClient();
}