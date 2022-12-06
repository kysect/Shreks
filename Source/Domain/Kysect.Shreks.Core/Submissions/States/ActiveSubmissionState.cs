using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.Models;
using Kysect.Shreks.Core.Tools;
using Kysect.Shreks.Core.ValueObject;

namespace Kysect.Shreks.Core.Submissions.States;

public class ActiveSubmissionState : ISubmissionState
{
    public SubmissionStateKind Kind => SubmissionStateKind.Active;
    public bool IsTerminalEffectiveState => false;

    public ISubmissionState MoveToRated(Fraction? rating, Points? extraPoints)
        => new CompletedSubmissionState();

    public ISubmissionState MoveToPointsUpdated(Fraction? rating, Points? extraPoints)
    {
        const string message = "Cannot update submission points. It has not been rated yet";
        throw new DomainInvalidOperationException(message);
    }

    public ISubmissionState MoveToBanned()
        => new BannedSubmissionState();

    public ISubmissionState MoveToActivated()
    {
        const string message = "Submission is already active";
        throw new DomainInvalidOperationException(message);
    }

    public ISubmissionState MoveToDeactivated()
        => new InactiveSubmissionState();

    public ISubmissionState MoveToDateUpdated(SpbDateTime newDate)
        => this;

    public ISubmissionState MoveToDeleted()
        => new DeletedSubmissionState();

    public ISubmissionState MoveToCompleted()
        => new CompletedSubmissionState();

    public ISubmissionState MoveToReviewed()
        => new ReviewedSubmissionState();
}