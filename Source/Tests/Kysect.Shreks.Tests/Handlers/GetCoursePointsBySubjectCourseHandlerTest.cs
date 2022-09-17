using FluentAssertions;
using Kysect.Shreks.Application.Handlers.Google;
using Kysect.Shreks.Tests.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using static Kysect.Shreks.Application.Abstractions.Google.Queries.GetCoursePointsBySubjectCourse;

namespace Kysect.Shreks.Tests.Handlers;

public class GetCoursePointsBySubjectCourseHandlerTest : ApplicationTestBase
{
    [Fact]
    public async Task Handle_Should_NotThrowException()
    {
        var handler = new GetCoursePointsBySubjectCourseHandler(Context, Mapper, NullLogger<GetCoursePointsBySubjectCourseHandler>.Instance);

        var subjectCourse = await Context.SubjectCourses.FirstAsync(x => x.Groups.Any());
        var response = await handler.Handle(new Query(subjectCourse.Id), CancellationToken.None);

        response.Points.Should().NotBeNull();
    }
}