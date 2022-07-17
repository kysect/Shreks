using FluentAssertions;
using Kysect.Shreks.Core.DeadlinePolicies;
using Kysect.Shreks.Core.Study;
using Xunit;

namespace Kysect.Shreks.Tests.DataAccess;

public class AssignmentTest : DataAccessTestBase
{
    [Fact]
    public async Task SaveChangesAsync_EntityAdded_NoExceptionThrown()
    {
        // Arrange
        var assignment = CreateAssignment();

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
        var assignment = CreateAssignment();

        // Act
        await Context.Assignments.AddAsync(assignment);
        await Context.SaveChangesAsync();

        // Assert
        Func<Task<Assignment?>> fetchFunction = async () => await Context.Assignments.FindAsync(assignment.Id);

        var fetchedAssignment = await fetchFunction.Should().NotThrowAsync();

        fetchedAssignment.Subject.Should().NotBeNull();
        fetchedAssignment.Subject!.DeadlinePolicies.Should().HaveCount(assignment.DeadlinePolicies.Count);
    }

    private static Assignment CreateAssignment()
    {
        var assignment = new Assignment("Assignment", 2, 10);

        var absolutePolicy = new AbsoluteDeadlinePolicy(TimeSpan.Zero, 10);
        var fractionPolicy = new FractionDeadlinePolicy(TimeSpan.Zero, 0.5);
        var cappingPolicy = new CappingDeadlinePolicy(TimeSpan.Zero, 3);

        assignment.AddDeadlinePolicy(absolutePolicy);
        assignment.AddDeadlinePolicy(fractionPolicy);
        assignment.AddDeadlinePolicy(cappingPolicy);

        return assignment;
    }
}