using FluentAssertions;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Seeding.EntityGenerators;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Kysect.Shreks.Tests.DataAccess;

public class StudentGroupTest : DataAccessTestBase
{
    private readonly IEntityGenerator<StudentGroup> _studentGroupGenerator;

    public StudentGroupTest()
    {
        _studentGroupGenerator = Provider.GetRequiredService<IEntityGenerator<StudentGroup>>();
    }
    
    [Fact]
    public async Task SaveChangesAsync_EntityAdded_NoExceptionThrown()
    {
        // Arrange
        var studentGroup = _studentGroupGenerator.Generate();

        // Act
        await Context.StudentGroups.AddAsync(studentGroup);

        // Assert
        Func<Task<int>> saveFunction = () => Context.SaveChangesAsync();

        await saveFunction.Should().NotThrowAsync();
    }

    [Fact]
    public async Task SaveChangesAsync_EntityFetched_NoExceptionThrown()
    {
        // Arrange
        var studentGroup = _studentGroupGenerator.Generate();

        // Act
        await Context.StudentGroups.AddAsync(studentGroup);
        await Context.SaveChangesAsync();

        // Assert
        Func<Task<StudentGroup?>> fetchFunction = async () => await Context.StudentGroups.FindAsync(studentGroup.Id);

        var fetchedAssignment = await fetchFunction.Should().NotThrowAsync();

        fetchedAssignment.Subject.Should().NotBeNull();
    }
}