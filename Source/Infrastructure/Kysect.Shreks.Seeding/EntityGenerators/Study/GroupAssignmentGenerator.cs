using Bogus;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Seeding.Options;

namespace Kysect.Shreks.Seeding.EntityGenerators;

public class GroupAssignmentGenerator : EntityGeneratorBase<GroupAssignment>
{
    private readonly IEntityGenerator<Assignment> _assignmentGenerator;
    private readonly Faker _faker;
    private readonly IEntityGenerator<StudentGroup> _groupGenerator;

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
        int groupCount = _groupGenerator.GeneratedEntities.Count;
        int assignmentCount = _assignmentGenerator.GeneratedEntities.Count;

        int groupNumber = index / assignmentCount;
        int assignmentNumber = index % assignmentCount;

        if (groupNumber >= groupCount)
            throw new IndexOutOfRangeException("The group index is greater than the number of groups.");

        StudentGroup group = _groupGenerator.GeneratedEntities[groupNumber];
        Assignment assignment = _assignmentGenerator.GeneratedEntities[assignmentNumber];

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