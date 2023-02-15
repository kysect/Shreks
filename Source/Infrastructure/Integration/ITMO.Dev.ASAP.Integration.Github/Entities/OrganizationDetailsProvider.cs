using ITMO.Dev.ASAP.Application.GithubWorkflow.Abstractions.Client;
using ITMO.Dev.ASAP.Application.GithubWorkflow.Abstractions.Providers;
using ITMO.Dev.ASAP.Integration.Github.Exceptions;
using Octokit;

namespace ITMO.Dev.ASAP.Integration.Github.Entities;

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

        IReadOnlyList<User> owners = await client.Organization.Member.GetAll(
            organizationName,
            OrganizationMembersRole.Admin);

        return owners.Select(u => u.Login).ToArray();
    }

    public async Task<IReadOnlyCollection<string>> GetOrganizationTeamMembers(string organizationName, string teamName)
    {
        GitHubClient client = await _clientProvider.GetClient(organizationName);
        IReadOnlyList<Team> teams = await client.Organization.Team.GetAll(organizationName);

        Team? team = teams.FirstOrDefault(t => t.Name == teamName);

        if (team is null)
            throw new InfrastructureInvalidOperationException("Cannot find team with name {teamName}");

        IReadOnlyList<User> teamMembers = await client.Organization.Team.GetAllMembers(team.Id);

        return teamMembers.Select(u => u.Login).ToArray();
    }
}