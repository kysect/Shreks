using FluentAssertions;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.Seeding.EntityGenerators;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Kysect.Shreks.Tests.DataAccess;

public class SubmissionTest : DataAccessTestBase
{
    private readonly IEntityGenerator<Submission> _submissionGenerator;

    public SubmissionTest()
    {
        var studentGenerator = Provider.GetRequiredService<IEntityGenerator<Student>>();
        var _ = studentGenerator.GeneratedEntities;

        _submissionGenerator = Provider.GetRequiredService<IEntityGenerator<Submission>>();
    }

    [Fact]
    public async Task SaveChangesAsync_EntityAdded_NoExceptionThrown()
    {
        // Arrange
        var submission = _submissionGenerator.Generate();

        // Act
        await Context.Submissions.AddAsync(submission);

        // Assert
        Func<Task<int>> saveFunction = () => Context.SaveChangesAsync();

        await saveFunction.Should().NotThrowAsync();
    }

    [Fact]
    public async Task SaveChangesAsync_EntityFetched_NoExceptionThrown()
    {
        // Arrange
        var submission = _submissionGenerator.Generate();

        // Act
        await Context.Submissions.AddAsync(submission);
        await Context.SaveChangesAsync();

        // Assert
        Func<Task<Submission?>> fetchFunction = async () => await Context.Submissions.FindAsync(submission.Id);

        var fetchedSubmission = await fetchFunction.Should().NotThrowAsync();

        fetchedSubmission.Subject.Should().NotBeNull();
        fetchedSubmission.Subject!.SubmissionDateTime.Should().Be(submission.SubmissionDateTime);
    }
}