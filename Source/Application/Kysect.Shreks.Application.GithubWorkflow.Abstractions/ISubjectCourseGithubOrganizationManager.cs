namespace Kysect.Shreks.Application.GithubWorkflow.Abstractions;

public interface ISubjectCourseGithubOrganizationManager
{
    Task UpdateOrganizations(CancellationToken cancellationToken);
}