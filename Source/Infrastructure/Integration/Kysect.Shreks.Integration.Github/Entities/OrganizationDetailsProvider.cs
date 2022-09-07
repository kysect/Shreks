using Kysect.Shreks.Application.GithubWorkflow.Abstractions;
using Kysect.Shreks.Integration.Github.Client;
using Octokit;

namespace Kysect.Shreks.Integration.Github.Entities;

public class OrganizationDetailsProvider : IOrganizationDetailsProvider
{
    private readonly IOrganizationGithubClientProvider _clientProvider;

    public OrganizationDetailsProvider(IOrganizationGithubClientProvider clientProvider)
    {
        _clientProvider = clientProvider;
    }

    public async Task<IReadOnlyCollection<string>> GetOrganizationOwners(string organizationName)
    {
        IGitHubClient client = await _clientProvider.GetClient(organizationName);
        IReadOnlyList<User> owners = await client.Organization.Member.GetAll(organizationName, OrganizationMembersRole.Admin);
        return owners.Select(u => u.Login).ToList();
    }
}