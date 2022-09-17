using FluentAssertions;
using Kysect.Shreks.Application.DatabaseContextExtensions;
using Kysect.Shreks.Core.SubjectCourseAssociations;
using Kysect.Shreks.Core.UserAssociations;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.Tests.Application;
using Xunit;

namespace Kysect.Shreks.Tests.GithubWorkflow;

public class GithubSubjectCourseAssociationTest : GithubWorkflowTestBase
{
    public GithubSubjectCourseAssociationTest()
    {
    }

    [Fact]
    public async Task GetSubjectCourseGithubUser_StudentAssociationExists_AssociationShouldReturn()
    {
        (GithubSubjectCourseAssociation subjectCourseAssociation, Student student) = await TestContextGenerator.Create();
        GithubUserAssociation githubUserAssociation = student.User.Associations.OfType<GithubUserAssociation>().First();

        // Assert
        IEnumerable<string> organizationUsers = Context
            .SubjectCourses
            .GetAllGithubUsers(subjectCourseAssociation.SubjectCourse.Id)
            .Result
            .Select(a => a.GithubUsername);

        organizationUsers.Should().Contain(githubUserAssociation.GithubUsername);
    }
}