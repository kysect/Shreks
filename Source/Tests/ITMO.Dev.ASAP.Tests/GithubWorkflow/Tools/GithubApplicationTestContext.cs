using ITMO.Dev.ASAP.Application.GithubWorkflow.Abstractions.Models;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.Core.SubjectCourseAssociations;
using ITMO.Dev.ASAP.Core.UserAssociations;
using ITMO.Dev.ASAP.Core.Users;

namespace ITMO.Dev.ASAP.Tests.GithubWorkflow.Tools;

public record GithubApplicationTestContext(
    GithubSubjectCourseAssociation SubjectCourseAssociation,
    Student Student)
{
    public GithubUserAssociation GetStudentGithubAssociation()
    {
        return Student.User.Associations.OfType<GithubUserAssociation>().First();
    }

    public Assignment GetAssignment()
    {
        return SubjectCourseAssociation.SubjectCourse.Assignments.First();
    }

    public GithubPullRequestDescriptor CreateStudentPullRequestDescriptor()
    {
        GithubUserAssociation githubUserAssociation = GetStudentGithubAssociation();
        Assignment assignment = GetAssignment();

        return new GithubPullRequestDescriptor(
            githubUserAssociation.GithubUsername,
            string.Empty,
            SubjectCourseAssociation.GithubOrganizationName,
            githubUserAssociation.GithubUsername,
            assignment.ShortName,
            1);
    }
}