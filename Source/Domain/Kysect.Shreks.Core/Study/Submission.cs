using Kysect.Shreks.Core.ValueObject;
using RichEntity.Annotations;

namespace Kysect.Shreks.Core.Study;

public partial class Submission : IEntity<Guid>
{
    public Submission(StudentAssignment studentAssignment, DateTime submissionDateTime)
        : this(Guid.NewGuid())
    {
        Points = Points.None;
        StudentAssignment = studentAssignment;
        SubmissionDateTime = submissionDateTime;
    }

    public DateTime SubmissionDateTime { get; set; }
    public virtual Points Points { get; set; }
    public virtual StudentAssignment StudentAssignment { get; protected init; }
}