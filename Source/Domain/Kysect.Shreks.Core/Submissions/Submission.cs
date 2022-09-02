using Kysect.Shreks.Common.Exceptions;
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
        DateTime submissionDate,
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
    
    public DateTime SubmissionDate { get; set; }
    public DateOnly SubmissionDateOnly => new DateOnly(SubmissionDate.Year, SubmissionDate.Month, SubmissionDate.Day);

    public virtual Student Student { get; protected init; }

    public virtual GroupAssignment GroupAssignment { get; protected init; }

    public SubmissionState State
    {
        get => _state;
        set => SetState(value);
    }

    public string Payload { get; set; }

    public Fraction? Rating { get; private set; }

    public Points? ExtraPoints { get; set; }

    public Points? Points => Rating is null ? default : GroupAssignment.Assignment.MaxPoints * Rating;

    public bool IsRated => Rating is not null;

    public virtual IReadOnlyCollection<SubmissionAssociation> Associations => _associations;

    public override string ToString() => $"{Code} ({Id})";

    public void Rate(Fraction? rating, Points? extraPoints)
    {
        if (State is not SubmissionState.Active and not SubmissionState.Completed)
        {
            string message = $"Cannot update submission points. Submission state: {State}.";
            throw new DomainInvalidOperationException(message);
        }

        if (rating is null && extraPoints is null)
        {
            const string ratingName = nameof(rating);
            const string extraPointsName = nameof(extraPoints);
            const string message = $"Cannot update submission points, both {ratingName} and {extraPointsName} are null.";
            throw new DomainInvalidOperationException(message);
        }

        if (rating is not null)
            Rating = rating;

        if (extraPoints is not null)
            ExtraPoints = extraPoints;

        _state = SubmissionState.Completed;
    }

    protected void AddAssociation(SubmissionAssociation association)
    {
        ArgumentNullException.ThrowIfNull(association);

        if (!_associations.Add(association))
            throw new DomainInvalidOperationException($"Submission {this} already has association {association}");
    }

    private void SetState(SubmissionState state)
    {
        if (_state is SubmissionState.Completed && state != SubmissionState.Completed)
            throw new DomainInvalidOperationException($"Submission {this} is already completed");

        if (_state is SubmissionState.Deleted)
            throw new DomainInvalidOperationException($"Submission {this} is already deleted");

        _state = state;
    }
}