using Kysect.Shreks.Core.Exceptions;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.Core.ValueObject;
using RichEntity.Annotations;

namespace Kysect.Shreks.Core.Study;

public partial class Submission : IEntity<Guid>
{
    private Points _points;

    public Submission(Student student, Assignment assignment, DateTime submissionDateTime) : this(Guid.NewGuid())
    {
        SubmissionDateTime = submissionDateTime;
        Student = student;
        Assignment = assignment;
        _points = Points.None;
    }

    public DateTime SubmissionDateTime { get; set; }

    public virtual Student Student { get; protected init; }

    public virtual Assignment Assignment { get; protected init; }

    public Points Points
    {
        get => _points;
        set => SetPoints(value);
    }

    private void SetPoints(Points points)
    {
        if (points < Assignment.MinPoints || Assignment.MaxPoints < points)
        {
            var message = $"Cannot rate submission for assignment {Assignment} with points {points}";
            throw new DomainInvalidOperationException(message);
        }

        _points = points;
    }
}