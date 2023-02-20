using ITMO.Dev.ASAP.Application.GithubWorkflow.Abstractions;
using ITMO.Dev.ASAP.Application.GithubWorkflow.Abstractions.Client;
using ITMO.Dev.ASAP.Application.GithubWorkflow.Abstractions.Models;
using Microsoft.Extensions.Logging;
using Octokit;

namespace ITMO.Dev.ASAP.Integration.Github.Invites;

public class SubjectCourseGithubOrganizationRepositoryManager : ISubjectCourseGithubOrganizationRepositoryManager
{
    private readonly IOrganizationGithubClientProvider _clientProvider;
    private readonly ILogger<SubjectCourseGithubOrganizationRepositoryManager> _logger;

    public SubjectCourseGithubOrganizationRepositoryManager(
        IOrganizationGithubClientProvider clientProvider,
        ILogger<SubjectCourseGithubOrganizationRepositoryManager> logger)
    {
        _clientProvider = clientProvider;
        _logger = logger;
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

        _logger.LogInformation(
            "Creating repository {OrganizationName}/{RepositoryName} from {Template}",
            organization,
            newRepositoryName,
            templateName);

        await client.Repository.Generate(
            organization,
            templateName,
            userRepositoryFromTemplate);
    }

    public async Task<AddPermissionResult> AddUserPermission(
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

        if (invitation is null)
        {
            bool isCollaborator = await client.Repository.Collaborator.IsCollaborator(
                organization,
                repositoryName,
                username);

            if (isCollaborator)
                return AddPermissionResult.AlreadyCollaborator;

            _logger.LogInformation(
                "Adding permission {Permission} for {Username} in {OrganizationName}/{RepositoryName}",
                permission,
                username,
                organization,
                username);

            await client.Repository.Collaborator.Add(
                organization,
                repositoryName,
                username,
                new CollaboratorRequest(permission));

            return AddPermissionResult.Invited;
        }

        if (DateTimeOffset.UtcNow.Subtract(invitation.CreatedAt) < TimeSpan.FromDays(7))
            return AddPermissionResult.Pending;

        _logger.LogInformation(
            "Invitation for {Username} in {OrganizationName}/{RepositoryName} is expired, renewing",
            username,
            organization,
            repositoryName);

        Repository repository = await client.Repository.Get(organization, repositoryName);
        await client.Repository.Invitation.Delete(repository.Id, invitation.Id);

        await client.Repository.Collaborator.Add(
            organization,
            repositoryName,
            username,
            new CollaboratorRequest(permission));

        return AddPermissionResult.ReInvited;
    }

    public async Task AddTeamPermission(
        string organization,
        string repositoryName,
        Team team,
        Permission permission)
    {
        GitHubClient client = await _clientProvider.GetClient(organization);

        _logger.LogInformation(
            "Adding permission {Permission} for {Team} in {OrganizationName}/{RepositoryName}",
            permission,
            team.Name,
            organization,
            repositoryName);

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