using FluentAssertions;
using Kysect.Shreks.Application.DatabaseContextExtensions;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.SubjectCourseAssociations;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.Seeding.EntityGenerators;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Kysect.Shreks.Tests.DataAccess;

public class GithubSubjectCourseAssociationTest : DataAccessTestBase
{
    private readonly IEntityGenerator<GithubSubjectCourseAssociation> _subjectCourseAssociationGenerator;
    private readonly IEntityGenerator<StudentGroup> _studentGroupGenerator;
    private readonly IEntityGenerator<User> _userGenerator;

    public GithubSubjectCourseAssociationTest()
    {
        _subjectCourseAssociationGenerator = Provider.GetRequiredService<IEntityGenerator<GithubSubjectCourseAssociation>>();
        _userGenerator = Provider.GetRequiredService<IEntityGenerator<User>>();
        _studentGroupGenerator = Provider.GetRequiredService<IEntityGenerator<StudentGroup>>();
    }

    [Fact]
    public async Task GetSubjectCourseGithubUser_StudentAssociationExists_AssociationShouldReturn()
    {
        // Arrange
        string userAssociation = Guid.NewGuid().ToString();

        User user = _userGenerator.Generate();
        StudentGroup group = _studentGroupGenerator.Generate();
        GithubSubjectCourseAssociation subjectCourseAssociation = _subjectCourseAssociationGenerator.Generate();

        var student = new Student(user, group);
        student.AddGithubAssociation(userAssociation);
        subjectCourseAssociation.SubjectCourse.AddGroup(group);
        group.AddStudent(student);

        // Act
        await Context.Users.AddAsync(user);
        await Context.Students.AddAsync(student);
        await Context.StudentGroups.AddAsync(group);
        await Context.SubjectCourseAssociations.AddAsync(subjectCourseAssociation);
        await Context.SaveChangesAsync();

        // Assert
        IEnumerable<string> organizationUsers = Context
            .SubjectCourses
            .GetAllGithubUsers(subjectCourseAssociation.Id)
            .Result
            .Select(a => a.GithubUsername);

        organizationUsers.Should().Contain(userAssociation);
    }
}