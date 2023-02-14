using FluentAssertions;
using ITMO.Dev.ASAP.Application.Abstractions.Formatters;
using ITMO.Dev.ASAP.Application.Abstractions.SubjectCourses;
using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.Application.Services;
using ITMO.Dev.ASAP.Core.Study;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Services;

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