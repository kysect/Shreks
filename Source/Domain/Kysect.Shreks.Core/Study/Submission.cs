using Kysect.Shreks.Core.Users;
using Kysect.Shreks.Core.ValueObject;
using RichEntity.Annotations;

namespace Kysect.Shreks.Core.Study;

public partial class Submission : IEntity<Guid>
{
    public Submission(Student student, Assignment assignment, DateTime submissionDateTime) : this(Guid.NewGuid())
    {
        SubmissionDateTime = submissionDateTime;
        Points = Points.None;
        Student = student;
        Assignment = assignment;
    }

    public DateTime SubmissionDateTime { get; set; }

    public Points Points { get; set; }

    public virtual Student Student { get; protected init; }

    public virtual Assignment Assignment { get; protected init; }
}