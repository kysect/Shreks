using FluentAssertions;
using Kysect.Shreks.Application.Abstractions.Students;
using Kysect.Shreks.Application.Handlers.Students;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Tests.Application;
using Xunit;

namespace Kysect.Shreks.Tests.Handlers;

public class StudentHandlerTest : ApplicationTestBase
{
    [Fact]
    public async Task Handle_Should_NoThrow()
    {
        SubjectCourse subjectCourse = Context.SubjectCourses
            .First(sc => sc.Groups.Any(g => g.StudentGroup.Students.Any()));
        var query = new GetStudentsBySubjectCourseId.Query(subjectCourse.Id);
        var handler = new GetStudentsBySubjectCourseIdHandler(Context, Mapper);
        GetStudentsBySubjectCourseId.Response handle = await handler.Handle(query, CancellationToken.None);

        handle.Students.Should().NotBeEmpty();
    }
}