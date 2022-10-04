using Kysect.Shreks.Application.GithubWorkflow.Abstractions.Models;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.SubjectCourseAssociations;
using Kysect.Shreks.Core.UserAssociations;
using Kysect.Shreks.Core.Users;

namespace Kysect.Shreks.Tests.GithubWorkflow.Tools;

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