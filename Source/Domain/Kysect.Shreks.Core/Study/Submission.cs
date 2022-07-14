using Kysect.Shreks.Core.ValueObject;
using RichEntity.Annotations;

namespace Kysect.Shreks.Core.Study;

public partial class Submission : IEntity<Guid>
{
    public Submission(StudentAssignment studentAssignment)
        : this(Guid.NewGuid())
    {
        Points = Points.None;
        StudentAssignment = studentAssignment;
    }

    public DateTime SubmissionDateTime { get; protected init; }
    public virtual Points Points { get; set; }
    public virtual StudentAssignment StudentAssignment { get; protected init; }
}