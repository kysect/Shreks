namespace Kysect.Shreks.Application.GithubWorkflow.Abstractions;

public interface ISubjectCourseGithubOrganizationInviteSender
{
    Task Invite(string organizationName, IReadOnlyCollection<string> usernames);
}