using FluentAssertions;
using Kysect.Shreks.Application.Abstractions.SubjectCourses;
using Kysect.Shreks.Application.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Kysect.Shreks.Tests.Services;

public class SubjectCourseServiceTest : ServicesTestsBase
{
    private readonly ISubjectCourseService _service;

    public SubjectCourseServiceTest()
    {
        _service = new SubjectCourseService(Context, Mapper);
    }

    [Fact]
    public async Task CalculatePointsAsync_Should_ReturnPoints()
    {
        var course = await Context.SubjectCourses
            .Where(x => x.Assignments
                .SelectMany(xx => xx.GroupAssignments)
                .SelectMany(xx => xx.Submissions)
                .Any())
            .FirstAsync();

        var points = await _service.CalculatePointsAsync(course.Id, default);

        points.StudentsPoints.Should().NotBeEmpty();
    }
}