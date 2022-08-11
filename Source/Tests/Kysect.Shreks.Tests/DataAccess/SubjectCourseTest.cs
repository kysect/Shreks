using FluentAssertions;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Seeding.EntityGenerators;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Kysect.Shreks.Tests.DataAccess;

public class SubjectCourseTest : DataAccessTestBase
{
    private readonly IEntityGenerator<SubjectCourse> _subjectCourseGenerator;

    public SubjectCourseTest()
    {
        _subjectCourseGenerator = Provider.GetRequiredService<IEntityGenerator<SubjectCourse>>();
    }
    
    [Fact]
    public async Task SaveChangesAsync_EntityAdded_NoExceptionThrown()
    {
        // Arrange
        var subjectCourse = _subjectCourseGenerator.Generate();

        // Act
        await Context.SubjectCourses.AddAsync(subjectCourse);

        // Assert
        Func<Task<int>> saveFunction = () => Context.SaveChangesAsync();

        await saveFunction.Should().NotThrowAsync();
    }

    [Fact]
    public async Task SaveChangesAsync_EntityFetched_NoExceptionThrown()
    {
        // Arrange
        var subjectCourse = _subjectCourseGenerator.Generate();

        // Act
        await Context.SubjectCourses.AddAsync(subjectCourse);
        await Context.SaveChangesAsync();

        // Assert
        Func<Task<SubjectCourse?>> fetchFunction = async () => await Context.SubjectCourses.FindAsync(subjectCourse.Id);

        var fetchedSubjectCourse = await fetchFunction.Should().NotThrowAsync();

        fetchedSubjectCourse.Subject.Should().NotBeNull();
        fetchedSubjectCourse.Subject!.Assignments.Should().HaveCount(subjectCourse.Assignments.Count);
    }
}