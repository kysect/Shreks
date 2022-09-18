using FluentAssertions;
using Kysect.Shreks.Application.DatabaseContextExtensions;
using Kysect.Shreks.Core.SubjectCourseAssociations;
using Kysect.Shreks.Core.UserAssociations;
using Kysect.Shreks.Core.Users;
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

        IReadOnlyCollection<GithubUserAssociation> githubUserAssociations = await Context
            .SubjectCourses
            .GetAllGithubUsers(subjectCourseAssociation.SubjectCourse.Id);

        IEnumerable<string> organizationUsers = githubUserAssociations.Select(a => a.GithubUsername);

        organizationUsers.Should().Contain(githubUserAssociation.GithubUsername);
    }
}