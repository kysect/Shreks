using FluentAssertions;
using Kysect.Shreks.Application.Handlers.Students;
using Kysect.Shreks.Core.Study;
using Microsoft.EntityFrameworkCore;
using Xunit;
using static Kysect.Shreks.Application.Abstractions.Students.GetGithubUsernamesForSubjectCourse;

namespace Kysect.Shreks.Tests.Application;

public class GithubWorkerTest : ApplicationTestBase
{
    //TODO: check after #83 implemented
    [Fact]
    public async Task GetStudentGithubsForSubjectId_NoExceptionThrown()
    {
        var handler = new GetGithubUsernamesForSubjectCourseHandler(Context);

        SubjectCourse subjectCourse = await Context.SubjectCourses.FirstAsync();

        Response response = await handler.Handle(new Query(subjectCourse.Id), CancellationToken.None);

        response.StudentGithubUsernames.Should().NotBeNull();
    }

}