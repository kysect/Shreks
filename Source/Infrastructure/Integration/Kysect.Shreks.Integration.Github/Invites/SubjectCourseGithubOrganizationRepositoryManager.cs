using Kysect.Shreks.Application.GithubWorkflow.Abstractions;
using Kysect.Shreks.Application.GithubWorkflow.Abstractions.Client;
using Octokit;

namespace Kysect.Shreks.Integration.Github.Invites;

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
        return repositories
            .Select(repository => repository.Name)
            .ToList();
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

    public async Task AddAdminPermission(string organization, string repositoryName, string username)
    {
        GitHubClient client = await _clientProvider.GetClient(organization);

        await client.Repository.Collaborator.Add(
            organization,
            repositoryName,
            username,
            new CollaboratorRequest(Permission.Admin));
    }
}