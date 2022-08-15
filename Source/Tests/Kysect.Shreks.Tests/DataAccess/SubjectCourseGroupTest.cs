using FluentAssertions;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Seeding.EntityGenerators;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Kysect.Shreks.Tests.DataAccess;

public class SubjectCourseGroupTest : DataAccessTestBase
{
    private readonly IEntityGenerator<SubjectCourseGroup> _subjectCourseGroupGenerator;

    public SubjectCourseGroupTest()
    {
        _subjectCourseGroupGenerator = Provider.GetRequiredService<IEntityGenerator<SubjectCourseGroup>>();
    }
    
    [Fact]
    public async Task SaveChangesAsync_EntityAdded_NoExceptionThrown()
    {
        // Arrange
        var subjectCourseGroup = _subjectCourseGroupGenerator.Generate();

        // Act
        await Context.SubjectCourseGroups.AddAsync(subjectCourseGroup);

        // Assert
        Func<Task<int>> saveFunction = () => Context.SaveChangesAsync();

        await saveFunction.Should().NotThrowAsync();
    }

    [Fact]
    public async Task SaveChangesAsync_EntityFetched_NoExceptionThrown()
    {
        // Arrange
        var subjectCourseGroup = _subjectCourseGroupGenerator.Generate();

        // Act
        await Context.SubjectCourseGroups.AddAsync(subjectCourseGroup);
        await Context.SaveChangesAsync();

        // Assert
        Func<Task<SubjectCourseGroup?>> fetchFunction = async () => await Context.SubjectCourseGroups.FindAsync(subjectCourseGroup.SubjectCourseId, subjectCourseGroup.StudentGroupId);

        var fetchedAssignment = await fetchFunction.Should().NotThrowAsync();

        fetchedAssignment.Should().NotBeNull();
    }
}