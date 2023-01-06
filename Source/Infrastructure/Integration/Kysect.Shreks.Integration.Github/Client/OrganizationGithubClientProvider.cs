using Kysect.Shreks.Application.GithubWorkflow.Abstractions.Client;
using Octokit;

namespace Kysect.Shreks.Integration.Github.Client;

public class OrganizationGithubClientProvider : IOrganizationGithubClientProvider
{
    private readonly IGitHubClient _appClient;
    private readonly IInstallationClientFactory _clientFactory;

    public OrganizationGithubClientProvider(IGitHubClient appClient, IInstallationClientFactory clientFactory)
    {
        _appClient = appClient;
        _clientFactory = clientFactory;
    }

    public async Task<GitHubClient> GetClient(string organization)
    {
        Installation installation = await _appClient.GitHubApps.GetOrganizationInstallationForCurrent(organization);
        return _clientFactory.GetClient(installation.Id);
    }
}