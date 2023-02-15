using ITMO.Dev.ASAP.Application.GithubWorkflow.Abstractions.Models;
using Octokit;

namespace ITMO.Dev.ASAP.Application.GithubWorkflow.Abstractions;

public interface ISubjectCourseGithubOrganizationRepositoryManager
{
    Task<IReadOnlyCollection<string>> GetRepositories(string organization);

    Task<Team> GetTeam(string organization, string teamName);

    Task CreateRepositoryFromTemplate(string organization, string newRepositoryName, string templateName);

    Task<AddPermissionResult> AddUserPermission(string organization, string repositoryName, string username, Permission permission);

    Task AddTeamPermission(string organization, string repositoryName, Team team, Permission permission);

    Task<bool> IsRepositoryCollaborator(string organization, string repositoryName, string userName);
}