using Octokit;

namespace ITMO.Dev.ASAP.Application.GithubWorkflow.Abstractions;

public interface ISubjectCourseGithubOrganizationRepositoryManager
{
    Task<IReadOnlyCollection<string>> GetRepositories(string organization);

    Task CreateRepositoryFromTemplate(string organization, string newRepositoryName, string templateName);

    Task AddUserPermission(string organization, string repositoryName, string username, Permission permission);
}