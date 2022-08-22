using FluentAssertions;
using Kysect.Shreks.Application.Handlers.Google;
using Microsoft.EntityFrameworkCore;
using Xunit;
using static Kysect.Shreks.Application.Abstractions.Google.Queries.GetCoursePointsBySubjectCourse;

namespace Kysect.Shreks.Tests.Application;

public class GetCoursePointsBySubjectCourseHandlerTest : ApplicationTestBase
{
    [Fact]
    public async Task Handle_Should_NotThrowException()
    {
        var handler = new GetCoursePointsBySubjectCourseHandler(Context, Mapper);

        var subjectCourse = await Context.SubjectCourses.FirstAsync(x => x.Groups.Any());
        var response = await handler.Handle(new Query(subjectCourse.Id), CancellationToken.None);

        response.Points.Should().NotBeNull();
    }
}