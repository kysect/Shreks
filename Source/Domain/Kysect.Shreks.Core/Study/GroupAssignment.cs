using RichEntity.Annotations;

namespace Kysect.Shreks.Core.Study;

public partial class GroupAssignment : IEntity
{
    public GroupAssignment(StudentGroup group, Assignment assignment, DateOnly deadline)
        : this(groupId: group.Id, assignmentId: assignment.Id)
    {
        ArgumentNullException.ThrowIfNull(group);
        ArgumentNullException.ThrowIfNull(assignment);

        Group = group;
        Assignment = assignment;
        DeadlineDateTime = deadline.ToDateTime(TimeOnly.MinValue);
    }

    [KeyProperty]
    public virtual StudentGroup Group { get; protected init; }

    [KeyProperty]
    public virtual Assignment Assignment { get; protected init; }

    // TODO: WI-226
    public DateTime DeadlineDateTime { get; set; }
    public DateOnly Deadline => new DateOnly(DeadlineDateTime.Year, DeadlineDateTime.Month, DeadlineDateTime.Day);

    public override String ToString()
    {
        return $"Assignment: {Assignment}, Group: {Group}";
    }
}