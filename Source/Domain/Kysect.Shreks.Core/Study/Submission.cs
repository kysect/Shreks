using Kysect.Shreks.Core.Exceptions;
using Kysect.Shreks.Core.SubmissionAssociations;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.Core.ValueObject;
using RichEntity.Annotations;

namespace Kysect.Shreks.Core.Study;

public partial class Submission : IEntity<Guid>
{
    private SubmissionAssociation? _association;

    public Submission(Student student, Assignment assignment, DateOnly submissionDate)
        : this(Guid.NewGuid())
    {
        SubmissionDate = submissionDate;
        Student = student;
        Assignment = assignment;
        ExtraPoints = Points.None;
        Rating = Fraction.None;
    }

    public DateOnly SubmissionDate { get; set; }

    public virtual Student Student { get; protected init; }

    public virtual Assignment Assignment { get; protected init; }

    public virtual SubmissionAssociation? Association => _association;

    public Points ExtraPoints { get; set; }

    public Fraction Rating { get; set; }

    public Points Points => Assignment.MaxPoints * Rating;

    public void UpdateAssociation(SubmissionAssociation association)
    {
        ArgumentNullException.ThrowIfNull(association);
        _association = association;
    }

    public void RemoveAssociation()
    {
        if (_association is null)
            throw new DomainInvalidOperationException($"Association is not assigned to submission {Id}");
    }
}