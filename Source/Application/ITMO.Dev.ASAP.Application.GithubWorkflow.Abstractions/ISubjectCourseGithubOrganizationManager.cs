namespace ITMO.Dev.ASAP.Application.GithubWorkflow.Abstractions;

public interface ISubjectCourseGithubOrganizationManager
{
    Task UpdateOrganizations(CancellationToken cancellationToken);
}