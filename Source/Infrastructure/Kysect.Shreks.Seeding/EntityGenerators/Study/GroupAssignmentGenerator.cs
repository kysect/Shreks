using Bogus;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Seeding.Options;

namespace Kysect.Shreks.Seeding.EntityGenerators;

public class GroupAssignmentGenerator : EntityGeneratorBase<GroupAssignment>
{
    private readonly IEntityGenerator<StudentGroup> _groupGenerator;
    private readonly IEntityGenerator<Assignment> _assignmentGenerator;
    private readonly Faker _faker;

    public GroupAssignmentGenerator(
        EntityGeneratorOptions<GroupAssignment> options,
        IEntityGenerator<StudentGroup> groupGenerator,
        IEntityGenerator<Assignment> assignmentGenerator,
        Faker faker) 
        : base(options)
    {
        _groupGenerator = groupGenerator;
        _assignmentGenerator = assignmentGenerator;
        _faker = faker;
    }

    protected override GroupAssignment Generate(int index)
    {
        var assignmentCount = _assignmentGenerator.GeneratedEntities.Count;

        var groupNumber = index / assignmentCount;
        var assignmentNumber = index % assignmentCount;

        var group = _groupGenerator.GeneratedEntities[groupNumber];
        var assignment = _assignmentGenerator.GeneratedEntities[assignmentNumber];

        var groupAssignment = new GroupAssignment
        (
            group,
            assignment,
            DateOnly.FromDateTime(_faker.Date.Future())
        );
        
        assignment.AddGroupAssignment(groupAssignment);

        return groupAssignment;
    }
}