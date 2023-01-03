using Kysect.Shreks.Application.GithubWorkflow.Abstractions.Client;
using Octokit;

namespace Kysect.Shreks.Integration.Github.Client;

public class ServiceOrganizationGithubClientProvider : IServiceOrganizationGithubClientProvider
{
    private readonly IGitHubClient _appClient;
    private readonly IInstallationClientFactory _clientFactory;
    private readonly string _serviceOrganization;

    public ServiceOrganizationGithubClientProvider(
        IGitHubClient appClient,
        IInstallationClientFactory clientFactory,
        string serviceOrganization)
    {
        ArgumentNullException.ThrowIfNull(appClient);
        ArgumentNullException.ThrowIfNull(clientFactory);
        ArgumentNullException.ThrowIfNull(serviceOrganization);

        _appClient = appClient;
        _clientFactory = clientFactory;
        _serviceOrganization = serviceOrganization;
    }

    public async Task<GitHubClient> GetClient()
    {
        Installation installation =
            await _appClient.GitHubApps.GetOrganizationInstallationForCurrent(_serviceOrganization);
        return _clientFactory.GetClient(installation.Id);
    }
}