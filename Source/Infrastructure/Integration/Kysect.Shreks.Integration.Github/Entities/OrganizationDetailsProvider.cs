using Kysect.Shreks.Application.Abstractions.Github;
using Kysect.Shreks.Integration.Github.Client;
using Octokit;

namespace Kysect.Shreks.Integration.Github.Entities;

public class OrganizationDetailsProvider : IOrganizationDetailsProvider
{
    private readonly IGitHubClient _appClient;
    private readonly IInstallationClientFactory _clientFactory;

    public OrganizationDetailsProvider(IGitHubClient appClient, IInstallationClientFactory clientFactory)
    {
        _appClient = appClient;
        _clientFactory = clientFactory;
    }

    public async Task<IReadOnlyCollection<string>> GetOrganizationOwners(string organizationName)
    {
        Installation installation = await _appClient.GitHubApps.GetOrganizationInstallationForCurrent(organizationName);
        GitHubClient client = _clientFactory.GetClient(installation.Id);
        IReadOnlyList<User> owners = await client.Organization.Member.GetAll(organizationName, OrganizationMembersRole.Admin);
        return owners.Select(u => u.Login).ToList();
    }
}