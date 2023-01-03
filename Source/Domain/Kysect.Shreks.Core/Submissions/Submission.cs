using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.DeadlinePolicies;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.SubmissionAssociations;
using Kysect.Shreks.Core.Submissions.States;
using Kysect.Shreks.Core.Tools;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.Core.ValueObject;
using RichEntity.Annotations;

namespace Kysect.Shreks.Core.Submissions;

public abstract partial class Submission : IEntity<Guid>
{
    private readonly HashSet<SubmissionAssociation> _associations;

    protected Submission(
        int code,
        Student student,
        GroupAssignment groupAssignment,
        SpbDateTime submissionDate,
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
        State = new ActiveSubmissionState();
    }

    public int Code { get; protected init; }

    public string Payload { get; set; }

    public Fraction? Rating { get; private set; }

    public Points? ExtraPoints { get; set; }

    public SpbDateTime SubmissionDate { get; private set; }

    public virtual Student Student { get; protected init; }

    public virtual GroupAssignment GroupAssignment { get; protected init; }

    public virtual IReadOnlyCollection<SubmissionAssociation> Associations => _associations;

    public Points? Points => Rating is null ? default : GroupAssignment.Assignment.MaxPoints * Rating;

    /// <summary>
    ///     Gets points with deadline policy applied.
    /// </summary>
    public Points? EffectivePoints => GetEffectivePoints();

    /// <summary>
    ///     Gets points subtracted by deadline policy.
    /// </summary>
    public Points? PointPenalty => GetPointPenalty();

    public bool IsRated => Rating is not null;

    public DateOnly SubmissionDateOnly => SubmissionDate.AsDateOnly();

    public ISubmissionState State { get; private set; }

    public override string ToString()
    {
        return $"{Code} ({Id})";
    }

    public void Rate(Fraction? rating, Points? extraPoints)
    {
        if (rating is null && extraPoints is null)
        {
            const string ratingName = nameof(rating);
            const string extraPointsName = nameof(extraPoints);
            const string message =
                $"Cannot update submission points, both {ratingName} and {extraPointsName} are null.";
            throw new DomainInvalidOperationException(message);
        }

        State = State.MoveToRated(rating, extraPoints);

        if (rating is not null)
            Rating = rating;

        if (extraPoints is not null)
            ExtraPoints = extraPoints;
    }

    public void UpdatePoints(Fraction? rating, Points? extraPoints)
    {
        if (rating is null && extraPoints is null)
        {
            const string ratingName = nameof(rating);
            const string extraPointsName = nameof(extraPoints);
            const string message =
                $"Cannot update submission points, both {ratingName} and {extraPointsName} are null.";
            throw new DomainInvalidOperationException(message);
        }

        State = State.MoveToPointsUpdated(rating, extraPoints);

        if (rating is not null)
            Rating = rating;

        if (extraPoints is not null)
            ExtraPoints = extraPoints;
    }

    public void UpdateDate(SpbDateTime newDate)
    {
        State = State.MoveToDateUpdated(newDate);
        SubmissionDate = newDate;
    }

    public void Activate()
    {
        State = State.MoveToActivated();
    }

    public void Deactivate()
    {
        State = State.MoveToDeactivated();
    }

    public void Ban()
    {
        State = State.MoveToBanned();
    }

    public void Delete()
    {
        State = State.MoveToDeleted();
    }

    public void Complete()
    {
        State = State.MoveToCompleted();
    }

    public void MarkAsReviewed()
    {
        State = State.MoveToReviewed();
    }

    protected void AddAssociation(SubmissionAssociation association)
    {
        ArgumentNullException.ThrowIfNull(association);

        if (!_associations.Add(association))
            throw new DomainInvalidOperationException($"Submission {this} already has association {association}");
    }

    private Points? GetEffectivePoints()
    {
        if (Points is null)
            return null;

        Points points = Points.Value;
        DeadlinePolicy? deadlinePolicy = GetEffectiveDeadlinePolicy();

        if (deadlinePolicy is not null)
            points = deadlinePolicy.Apply(points);

        if (ExtraPoints is not null)
            points += ExtraPoints.Value;

        return points;
    }

    private Points? GetPointPenalty()
    {
        if (Points is null)
            return null;

        Points? deadlineAppliedPoints = GetEffectivePoints();

        if (deadlineAppliedPoints is null)
            return null;

        Points? penaltyPoints = Points - deadlineAppliedPoints;

        return penaltyPoints;
    }

    private DeadlinePolicy? GetEffectiveDeadlinePolicy()
    {
        DateOnly deadline = GroupAssignment.Deadline;

        if (SubmissionDateOnly <= deadline)
            return null;

        var submissionDeadlineOffset = TimeSpan.FromDays(SubmissionDateOnly.DayNumber - deadline.DayNumber);

        DeadlinePolicy? activeDeadlinePolicy = GroupAssignment
            .Assignment
            .SubjectCourse
            .DeadlinePolicies
            .Where(dp => dp.SpanBeforeActivation < submissionDeadlineOffset)
            .MaxBy(dp => dp.SpanBeforeActivation);

        return activeDeadlinePolicy;
    }
}