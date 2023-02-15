namespace ITMO.Dev.ASAP.Application.GithubWorkflow.Abstractions;

public interface ISubjectCourseGithubOrganizationInviteSender
{
    Task Invite(string organizationName, IReadOnlyCollection<string> usernames);
}