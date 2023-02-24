namespace ITMO.Dev.ASAP.Application.GithubWorkflow.Abstractions;

public interface ISubjectCourseGithubOrganizationManager
{
    Task UpdateOrganizationsAsync(CancellationToken cancellationToken);

    Task UpdateSubjectCourseOrganizationAsync(Guid subjectCourseId, CancellationToken cancellationToken);
}