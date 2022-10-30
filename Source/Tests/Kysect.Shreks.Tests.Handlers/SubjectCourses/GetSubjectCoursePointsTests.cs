using FluentAssertions;
using Kysect.Shreks.Application.Handlers.SubjectCourses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using static Kysect.Shreks.Application.Abstractions.SubjectCourses.Queries.GetSubjectCoursePoints;

namespace Kysect.Shreks.Tests.Handlers.SubjectCourses;

public class GetSubjectCoursePointsTests : HandlerTestBase
{
    [Fact]
    public async Task Handle_Should_NotThrowException()
    {
        var handler = new GetSubjectCoursePointsHandler(Context, Mapper, NullLogger<GetSubjectCoursePointsHandler>.Instance);

        var subjectCourse = await Context.SubjectCourses.FirstAsync(x => x.Groups.Any());
        var response = await handler.Handle(new Query(subjectCourse.Id), CancellationToken.None);

        response.Points.Should().NotBeNull();
    }
}