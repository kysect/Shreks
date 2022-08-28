using FluentAssertions;
using Kysect.Shreks.Core.Models;
using Kysect.Shreks.Core.Queue;
using Kysect.Shreks.Core.Queue.Evaluators;
using Kysect.Shreks.Core.Queue.Filters;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Kysect.Shreks.Tests.DataAccess;

public class SubmissionQueueTest : DataAccessTestBase
{
    [Fact]
    public async Task FindAsync_Should_FetchNonEmptyQueue()
    {
        // Arrange
        var filters = new[]
        {
            new SubmissionStateFilter(SubmissionState.Active),
        };

        var evaluators = new[]
        {
            new AssignmentDeadlineStateEvaluator(0, SortingOrder.Descending),
        };

        var queue = new SubmissionQueue(filters, evaluators);

        Context.SubmissionQueues.Add(queue);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();

        // Act
        var fetchedQueue = await Context.SubmissionQueues
            .Where(x => x.Id.Equals(queue.Id))
            .SingleAsync();

        // Assert
        fetchedQueue.Filters.Should().HaveCount(filters.Length);
        fetchedQueue.Evaluators.Should().HaveCount(evaluators.Length);
    }
}