using FluentAssertions;
using Kysect.Shreks.Application.Handlers.Google;
using Kysect.Shreks.Core.Study;
using Microsoft.EntityFrameworkCore;
using Xunit;
using static Kysect.Shreks.Application.Abstractions.Google.Queries.GetCoursePointsBySubjectCourse;

namespace Kysect.Shreks.Tests.Application;

public class GoogleWorkerTest : ApplicationTestBase
{
    [Fact]
    public async Task GetCoursePointsForSubjectId_NoExceptionThrown()
    {
        var handler = new GetCoursePointsBySubjectCourseHandler(Context);

        SubjectCourse subjectCourse = await Context.SubjectCourses.FirstAsync();

        Response response = await handler.Handle(new Query(subjectCourse.Id), CancellationToken.None);

        response.Points.Should().NotBeNull();
    }
}