using FluentAssertions;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Seeding.EntityGenerators;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Kysect.Shreks.Tests.DataAccess;

public class GroupAssignmentTest : DataAccessTestBase
{
    private readonly IEntityGenerator<GroupAssignment> _groupAssignmentGenerator;

    public GroupAssignmentTest()
    {
        _groupAssignmentGenerator = Provider.GetRequiredService<IEntityGenerator<GroupAssignment>>();
    }

    [Fact]
    public async Task SaveChangesAsync_EntityAdded_NoExceptionThrown()
    {
        var groupAssignment = _groupAssignmentGenerator.Generate();

        await Context.GroupAssignments.AddAsync(groupAssignment);
        
        Func<Task<int>> saveFunction = () => Context.SaveChangesAsync();

        await saveFunction.Should().NotThrowAsync();
    }
    
    [Fact]
    public async Task SaveChangesAsync_EntityFetched_NoExceptionThrown()
    {
        // Arrange
        var assignment = _groupAssignmentGenerator.Generate();

        // Act
        await Context.GroupAssignments.AddAsync(assignment);
        await Context.SaveChangesAsync();

        // Assert
        Func<Task<Assignment?>> fetchFunction = async () => await Context.Assignments.FindAsync(assignment.AssignmentId);

        var fetchedAssignment = await fetchFunction.Should().NotThrowAsync();

        fetchedAssignment.Subject.Should().NotBeNull();
        fetchedAssignment.Subject!.DeadlinePolicies.Should().HaveCount(assignment.Assignment.DeadlinePolicies.Count);
    }
}