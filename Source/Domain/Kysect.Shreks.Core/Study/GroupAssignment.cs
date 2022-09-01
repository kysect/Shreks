using Kysect.Shreks.Core.Submissions;
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
        Deadline = deadline;
    }

    [KeyProperty]
    public virtual StudentGroup Group { get; protected init; }

    [KeyProperty]
    public virtual Assignment Assignment { get; protected init; }
    public DateOnly Deadline { get; set; }
    public virtual IReadOnlyCollection<Submission> Submissions { get; protected init; }

    public override String ToString()
    {
        return $"Assignment: {Assignment}, Group: {Group}";
    }
}