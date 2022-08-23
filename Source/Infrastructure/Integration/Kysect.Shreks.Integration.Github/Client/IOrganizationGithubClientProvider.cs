using Octokit;

namespace Kysect.Shreks.Integration.Github.Client;

public interface IOrganizationGithubClientProvider
{
    public Task<GitHubClient> GetClient(string organization);
}