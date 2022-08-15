using FluentAssertions;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Seeding.EntityGenerators;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Kysect.Shreks.Tests.DataAccess;

public class SubjectTest : DataAccessTestBase
{
    private readonly IEntityGenerator<Subject> _subjectGenerator;

    public SubjectTest()
    {
        _subjectGenerator = Provider.GetRequiredService<IEntityGenerator<Subject>>();
    }

    [Fact]
    public async Task SaveChangesAsync_EntityAdded_NoExceptionThrown()
    {
        // Arrange
        var subject = _subjectGenerator.Generate();

        // Act
        await Context.Subjects.AddAsync(subject);

        // Assert
        Func<Task<int>> saveFunction = () => Context.SaveChangesAsync();

        await saveFunction.Should().NotThrowAsync();
    }

    [Fact]
    public async Task SaveChangesAsync_EntityFetched_NoExceptionThrown()
    {
        // Arrange
        var subject = _subjectGenerator.Generate();

        // Act
        await Context.Subjects.AddAsync(subject);
        await Context.SaveChangesAsync();

        // Assert
        Func<Task<Subject?>> fetchFunction = async () => await Context.Subjects.FindAsync(subject.Id);

        var fetchedSubject = await fetchFunction.Should().NotThrowAsync();

        fetchedSubject.Subject.Should().NotBeNull();
        fetchedSubject.Subject!.Courses.Should().HaveCount(subject.Courses.Count);
    }
}