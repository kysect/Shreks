namespace Kysect.Shreks.Integration.Github.Invites;

public interface ISubjectCourseGithubOrganizationInviteSender
{
    Task Invite(string organizationName, IReadOnlyCollection<string> usernames);
}