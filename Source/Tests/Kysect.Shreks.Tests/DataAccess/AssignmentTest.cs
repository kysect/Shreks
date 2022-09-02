using FluentAssertions;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Seeding.EntityGenerators;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Kysect.Shreks.Tests.DataAccess;

public class AssignmentTest : DataAccessTestBase
{
    private readonly IEntityGenerator<Assignment> _assignmentGenerator;

    public AssignmentTest()
    {
        _assignmentGenerator = Provider.GetRequiredService<IEntityGenerator<Assignment>>();
    }

    [Fact]
    public async Task SaveChangesAsync_EntityAdded_NoExceptionThrown()
    {
        // Arrange
        var assignment = _assignmentGenerator.Generate();

        // Act
        await Context.Assignments.AddAsync(assignment);

        // Assert
        Func<Task<int>> saveFunction = () => Context.SaveChangesAsync();

        await saveFunction.Should().NotThrowAsync();
    }

    [Fact]
    public async Task SaveChangesAsync_EntityFetched_NoExceptionThrown()
    {
        // Arrange
        var assignment = _assignmentGenerator.Generate();

        // Act
        await Context.Assignments.AddAsync(assignment);
        await Context.SaveChangesAsync();

        // Assert
        Func<Task<Assignment?>> fetchFunction = async () => await Context.Assignments.FindAsync(assignment.Id);

        var fetchedAssignment = await fetchFunction.Should().NotThrowAsync();

        fetchedAssignment.Subject.Should().NotBeNull();
    }
}