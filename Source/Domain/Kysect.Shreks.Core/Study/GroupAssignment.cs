using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.Submissions;
using RichEntity.Annotations;

namespace Kysect.Shreks.Core.Study;

public partial class GroupAssignment : IEntity
{
    private readonly HashSet<Submission> _submissions;

    public GroupAssignment(StudentGroup group, Assignment assignment, DateOnly deadline)
        : this(groupId: group.Id, assignmentId: assignment.Id)
    {
        ArgumentNullException.ThrowIfNull(group);
        ArgumentNullException.ThrowIfNull(assignment);

        Group = group;
        Assignment = assignment;
        Deadline = deadline;

        _submissions = new HashSet<Submission>();
    }

    [KeyProperty] public virtual StudentGroup Group { get; protected init; }

    [KeyProperty] public virtual Assignment Assignment { get; protected init; }

    public DateOnly Deadline { get; set; }
    public virtual IReadOnlyCollection<Submission> Submissions => _submissions;

    public override string ToString()
    {
        return $"Assignment: {Assignment}, Group: {Group}";
    }

    public void AddSubmission(Submission submission)
    {
        if (submission.GroupAssignment.Equals(this) is false)
            throw new DomainInvalidOperationException($"Submission {submission} is not for assignment {this}");

        if (_submissions.Add(submission) is false)
            throw new DomainInvalidOperationException($"Submission {submission} already exists in assignment {this}");
    }
}