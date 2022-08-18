using Kysect.Shreks.Core.Users;
using Kysect.Shreks.Core.ValueObject;
using RichEntity.Annotations;

namespace Kysect.Shreks.Core.Study;

public partial class Submission : IEntity<Guid>
{
    public Submission(Student student, Assignment assignment, DateTime submissionDateTime, string payload) : this(Guid.NewGuid())
    {
        SubmissionDateTime = submissionDateTime;
        Student = student;
        Assignment = assignment;
        Payload = payload;
        ExtraPoints = Points.None;
        Rating = Rating.None;
    }

    public DateTime SubmissionDateTime { get; set; }

    public virtual Student Student { get; protected init; }

    public virtual Assignment Assignment { get; protected init; }

    public string Payload { get; set; }

    public Points ExtraPoints { get; set; }

    public Rating Rating { get; set; }

    public Points Points => Rating * Assignment.MaxPoints;
}