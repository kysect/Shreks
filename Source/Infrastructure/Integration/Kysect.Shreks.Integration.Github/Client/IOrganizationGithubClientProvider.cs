using Octokit;

namespace Kysect.Shreks.Integration.Github.Client;

public interface IOrganizationGithubClientProvider
{
    Task<GitHubClient> GetClient(string organization);
}