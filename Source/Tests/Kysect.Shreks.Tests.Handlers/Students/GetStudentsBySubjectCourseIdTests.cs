using FluentAssertions;
using Kysect.Shreks.Application.Contracts.Students.Queries;
using Kysect.Shreks.Application.Handlers.Students;
using Kysect.Shreks.Core.Study;
using Xunit;

namespace Kysect.Shreks.Tests.Handlers.Students;

public class GetStudentsBySubjectCourseIdTests : TestBase
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