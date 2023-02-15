using ITMO.Dev.ASAP.Application.GithubWorkflow.Abstractions;
using ITMO.Dev.ASAP.Application.GithubWorkflow.Abstractions.Client;
using Octokit;

namespace ITMO.Dev.ASAP.Integration.Github.Invites;

public class SubjectCourseGithubOrganizationRepositoryManager : ISubjectCourseGithubOrganizationRepositoryManager
{
    private readonly IOrganizationGithubClientProvider _clientProvider;

    public SubjectCourseGithubOrganizationRepositoryManager(IOrganizationGithubClientProvider clientProvider)
    {
        _clientProvider = clientProvider;
    }

    public async Task<IReadOnlyCollection<string>> GetRepositories(string organization)
    {
        GitHubClient client = await _clientProvider.GetClient(organization);
        IReadOnlyList<Repository> repositories = await client.Repository.GetAllForOrg(organization);

        return repositories.Select(repository => repository.Name).ToArray();
    }

    public async Task<Team> GetTeam(string organization, string teamName)
    {
        GitHubClient client = await _clientProvider.GetClient(organization);
        IReadOnlyList<Team> teams = await client.Organization.Team.GetAll(organization);

        return teams.Single(x => teamName.Equals(x.Name, StringComparison.OrdinalIgnoreCase));
    }

    public async Task CreateRepositoryFromTemplate(string organization, string newRepositoryName, string templateName)
    {
        GitHubClient client = await _clientProvider.GetClient(organization);

        var userRepositoryFromTemplate = new NewRepositoryFromTemplate(newRepositoryName)
        {
            Owner = organization, Description = null, Private = true,
        };

        await client.Repository.Generate(
            organization,
            templateName,
            userRepositoryFromTemplate);
    }

    public async Task AddUserPermission(
        string organization,
        string repositoryName,
        string username,
        Permission permission)
    {
        GitHubClient client = await _clientProvider.GetClient(organization);

        RepositoryInvitation invitation = await client.Repository.Collaborator.Invite(
            organization,
            repositoryName,
            username,
            new CollaboratorRequest(permission));

        if (DateTimeOffset.UtcNow.Subtract(invitation.CreatedAt) < TimeSpan.FromDays(7))
            return;

        Repository repository = await client.Repository.Get(organization, repositoryName);
        await client.Repository.Invitation.Delete(repository.Id, invitation.Id);

        await client.Repository.Collaborator.Add(
            organization,
            repositoryName,
            username,
            new CollaboratorRequest(permission));
    }

    public async Task AddTeamPermission(
        string organization,
        string repositoryName,
        Team team,
        Permission permission)
    {
        GitHubClient client = await _clientProvider.GetClient(organization);

        await client.Organization.Team.AddRepository(
            team.Id,
            organization,
            repositoryName,
            new RepositoryPermissionRequest(permission));
    }

    public async Task<bool> IsRepositoryCollaborator(string organization, string repositoryName, string userName)
    {
        GitHubClient client = await _clientProvider.GetClient(organization);
        Repository repository = await client.Repository.Get(organization, repositoryName);

        return await client.Repository.Collaborator.IsCollaborator(repository.Id, userName);
    }
}