using FluentAssertions;
using Kysect.Shreks.Core.Study;
using Microsoft.EntityFrameworkCore;
using Xunit;
using static Kysect.Shreks.Application.Abstractions.Students.GetGithubUsernamesForSubjectCourse;

namespace Kysect.Shreks.Tests.Application;

public class GetGithubUsernamesForSubjectCourseHandlerTest : ApplicationTestBase
{
    [Fact]
    public async Task Handle_Should_NotThrowException()
    {
        var handler = new Shreks.Application.Handlers.Students.GetGithubUsernamesForSubjectCourseHandler(Context);

        SubjectCourse subjectCourse = await Context.SubjectCourses.FirstAsync();

        Response response = await handler.Handle(new Query(subjectCourse.Id), CancellationToken.None);

        response.StudentGithubUsernames.Should().NotBeNull();
    }
}