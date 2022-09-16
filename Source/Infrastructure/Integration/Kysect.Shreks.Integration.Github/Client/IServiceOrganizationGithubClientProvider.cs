using Octokit;

namespace Kysect.Shreks.Integration.Github.Client;

public interface IServiceOrganizationGithubClientProvider
{
    Task<GitHubClient> GetClient();
}