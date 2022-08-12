using FluentAssertions;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.SubjectCourseAssociations;
using Kysect.Shreks.Seeding.EntityGenerators;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Kysect.Shreks.Tests.DataAccess;

public class SubjectCourseAssociationTest : DataAccessTestBase
{
    private readonly IEntityGenerator<SubjectCourseAssociation> _subjectCourseAssociationGenerator;

    public SubjectCourseAssociationTest()
    {
        _subjectCourseAssociationGenerator = Provider.GetRequiredService<IEntityGenerator<SubjectCourseAssociation>>();
    }
    
    [Fact]
    public async Task SaveChangesAsync_EntityAdded_NoExceptionThrown()
    {
        // Arrange
        var subjectCourseAssociation = _subjectCourseAssociationGenerator.Generate();

        // Act
        await Context.SubjectCourseAssociations.AddAsync(subjectCourseAssociation);

        // Assert
        Func<Task<int>> saveFunction = () => Context.SaveChangesAsync();

        await saveFunction.Should().NotThrowAsync();
    }

    [Fact]
    public async Task SaveChangesAsync_EntityFetched_NoExceptionThrown()
    {
        // Arrange
        var subjectCourseAssociation = _subjectCourseAssociationGenerator.Generate();

        // Act
        await Context.SubjectCourseAssociations.AddAsync(subjectCourseAssociation);
        await Context.SaveChangesAsync();

        // Assert
        Func<Task<SubjectCourseAssociation?>> fetchFunction = async () => await Context.SubjectCourseAssociations.FindAsync(subjectCourseAssociation.Id);

        var fetchedAssignment = await fetchFunction.Should().NotThrowAsync();

        fetchedAssignment.Subject.Should().NotBeNull();
        fetchedAssignment.Subject!.SubjectCourse.Id.Should().Be(subjectCourseAssociation.SubjectCourse.Id);
    }
}