namespace Kysect.Shreks.Integration.Github.Invites;

public interface ISubjectCourseGithubOrganizationRepositoryManager
{
    Task<IReadOnlyCollection<string>> GetRepositories(string organization);
    Task CreateRepositoryFromTemplate(string organization, string newRepositoryName, string templateName);
    Task AddAdminPermission(string organization, string repositoryName, string username);
}