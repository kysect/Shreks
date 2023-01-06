using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.Models;
using Kysect.Shreks.Core.Tools;
using Kysect.Shreks.Core.ValueObject;

namespace Kysect.Shreks.Core.Submissions.States;

public class ReviewedSubmissionState : ISubmissionState
{
    public SubmissionStateKind Kind => SubmissionStateKind.Reviewed;

    public bool IsTerminalEffectiveState => false;

    public ISubmissionState MoveToRated(Fraction? rating, Points? extraPoints)
    {
        return new CompletedSubmissionState();
    }

    public ISubmissionState MoveToPointsUpdated(Fraction? rating, Points? extraPoints)
    {
        const string message = "Cannot update submission points. It has not been rated yet";
        throw new DomainInvalidOperationException(message);
    }

    public ISubmissionState MoveToBanned()
    {
        return new BannedSubmissionState();
    }

    public ISubmissionState MoveToActivated()
    {
        const string message = "Cannot activate submission in reviewed state";
        throw new DomainInvalidOperationException(message);
    }

    public ISubmissionState MoveToDeactivated()
    {
        return new InactiveSubmissionState();
    }

    public ISubmissionState MoveToDateUpdated(SpbDateTime newDate)
    {
        return this;
    }

    public ISubmissionState MoveToDeleted()
    {
        return new DeletedSubmissionState();
    }

    public ISubmissionState MoveToCompleted()
    {
        return new CompletedSubmissionState();
    }

    public ISubmissionState MoveToReviewed()
    {
        const string message = "Submission is already reviewed";
        throw new DomainInvalidOperationException(message);
    }
}