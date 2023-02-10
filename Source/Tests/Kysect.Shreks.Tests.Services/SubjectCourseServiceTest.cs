using FluentAssertions;
using Kysect.Shreks.Application.Abstractions.Formatters;
using Kysect.Shreks.Application.Abstractions.SubjectCourses;
using Kysect.Shreks.Application.Dto.SubjectCourses;
using Kysect.Shreks.Application.Services;
using Kysect.Shreks.Core.Study;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Kysect.Shreks.Tests.Services;

public class SubjectCourseServiceTest : TestBase
{
    private readonly ISubjectCourseService _service;

    public SubjectCourseServiceTest()
    {
        _service = new SubjectCourseService(Context, new UserFullNameFormatter());
    }

    [Fact]
    public async Task CalculatePointsAsync_Should_ReturnPoints()
    {
        SubjectCourse course = await Context.SubjectCourses
            .Where(x => x.Assignments
                .SelectMany(xx => xx.GroupAssignments)
                .SelectMany(xx => xx.Submissions)
                .Any())
            .FirstAsync();

        SubjectCoursePointsDto points = await _service.CalculatePointsAsync(course.Id, default);

        points.StudentsPoints.Should().NotBeEmpty();
    }
}