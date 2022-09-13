namespace Kysect.Shreks.Application.GithubWorkflow.Abstractions;

public interface ISubjectCourseGithubOrganizationManager
{
    Task UpdateOrganizations(CancellationToken cancellationToken);
}

public interface ISubjectCourseGithubOrganizationInviteSender
{
    Task Invite(string organizationName, IReadOnlyCollection<string> usernames);
}