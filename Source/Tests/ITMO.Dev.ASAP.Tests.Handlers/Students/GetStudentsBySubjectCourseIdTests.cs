using FluentAssertions;
using ITMO.Dev.ASAP.Application.Contracts.Students.Queries;
using ITMO.Dev.ASAP.Application.Handlers.Students;
using ITMO.Dev.ASAP.Core.Study;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Handlers.Students;

public class GetStudentsBySubjectCourseIdTests : TestBase
{
    [Fact]
    public async Task Handle_Should_NoThrow()
    {
        SubjectCourse subjectCourse = Context.SubjectCourses
            .First(sc => sc.Groups.Any(g => g.StudentGroup.Students.Any()));

        var query = new GetStudentsBySubjectCourseId.Query(subjectCourse.Id);
        var handler = new GetStudentsBySubjectCourseIdHandler(Context);

        GetStudentsBySubjectCourseId.Response handle = await handler.Handle(query, CancellationToken.None);

        handle.Students.Should().NotBeEmpty();
    }
}