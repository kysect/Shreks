using Kysect.Shreks.Core.Exceptions;
using Kysect.Shreks.Core.SubmissionAssociations;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.Core.ValueObject;
using RichEntity.Annotations;

namespace Kysect.Shreks.Core.Study;

public partial class Submission : IEntity<Guid>
{
    private HashSet<SubmissionAssociation> _associations;
    
    public Submission(Student student, Assignment assignment, DateOnly submissionDate, string payload)
        : this(Guid.NewGuid())
    {
        SubmissionDate = submissionDate;
        Student = student;
        Assignment = assignment;
        Payload = payload;
        ExtraPoints = Points.None;
        Rating = Fraction.None;

        _associations = new HashSet<SubmissionAssociation>();
    }

    public DateOnly SubmissionDate { get; set; }

    public virtual Student Student { get; protected init; }

    public virtual Assignment Assignment { get; protected init; }

    public string Payload { get; set; }

    public Points ExtraPoints { get; set; }

    public Fraction Rating { get; set; }

    public Points Points => Assignment.MaxPoints * Rating;

    public virtual IReadOnlyCollection<SubmissionAssociation> Associations => _associations;
    
    public void AddAssociation(SubmissionAssociation association)
    {
        ArgumentNullException.ThrowIfNull(association);

        if (!_associations.Add(association))
            throw new DomainInvalidOperationException($"Submission {this} already has association {association}");
    }
    
    public void RemoveAssociation(SubmissionAssociation association)
    {
        ArgumentNullException.ThrowIfNull(association);

        if (!_associations.Remove(association))
            throw new DomainInvalidOperationException($"Submission {this} could not remove association {association}");
    }
}