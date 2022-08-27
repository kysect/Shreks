using Kysect.Shreks.Core.Exceptions;
using Kysect.Shreks.Core.Models;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.SubmissionAssociations;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.Core.ValueObject;
using RichEntity.Annotations;

namespace Kysect.Shreks.Core.Submissions;

public abstract partial class Submission : IEntity<Guid>
{
    private readonly HashSet<SubmissionAssociation> _associations;
    private SubmissionState _state;

    protected Submission(
        int code,
        Student student,
        GroupAssignment groupAssignment,
        DateOnly submissionDate,
        string payload)
        : this(Guid.NewGuid())
    {
        Code = code;
        SubmissionDate = submissionDate;
        Student = student;
        GroupAssignment = groupAssignment;
        Payload = payload;

        Rating = default;
        ExtraPoints = default;

        _associations = new HashSet<SubmissionAssociation>();
        _state = SubmissionState.Active;
    }

    public int Code { get; protected init; }

    public DateOnly SubmissionDate { get; set; }

    public virtual Student Student { get; protected init; }

    public virtual GroupAssignment GroupAssignment { get; protected init; }

    public SubmissionState State
    {
        get => _state;
        set => SetState(value);
    }

    public string Payload { get; set; }

    public Fraction? Rating { get; set; }

    public Points? ExtraPoints { get; set; }

    public Points? Points => Rating is null ? default : GroupAssignment.Assignment.MaxPoints * Rating;

    public bool IsRated => Rating is not null;

    public virtual IReadOnlyCollection<SubmissionAssociation> Associations => _associations;

    protected void AddAssociation(SubmissionAssociation association)
    {
        ArgumentNullException.ThrowIfNull(association);

        if (!_associations.Add(association))
            throw new DomainInvalidOperationException($"Submission {this} already has association {association}");
    }

    private void SetState(SubmissionState state)
    {
        if (_state is SubmissionState.Completed)
            throw new DomainInvalidOperationException($"Submission {this} is already completed");

        if (_state is SubmissionState.Deleted)
            throw new DomainInvalidOperationException($"Submission {this} is already deleted");

        _state = state;
    }
}