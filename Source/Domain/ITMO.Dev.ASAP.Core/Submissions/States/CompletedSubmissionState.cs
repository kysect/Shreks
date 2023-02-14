using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Core.Models;
using ITMO.Dev.ASAP.Core.Tools;
using ITMO.Dev.ASAP.Core.ValueObject;

namespace ITMO.Dev.ASAP.Core.Submissions.States;

public class CompletedSubmissionState : ISubmissionState
{
    public SubmissionStateKind Kind => SubmissionStateKind.Completed;

    public bool IsTerminalEffectiveState => true;

    public ISubmissionState MoveToRated(Fraction? rating, Points? extraPoints)
    {
        const string message = "Submission is already completed and cannot be rated again";
        throw new DomainInvalidOperationException(message);
    }

    public ISubmissionState MoveToPointsUpdated(Fraction? rating, Points? extraPoints)
    {
        return this;
    }

    public ISubmissionState MoveToBanned()
    {
        return new BannedSubmissionState();
    }

    public ISubmissionState MoveToActivated()
    {
        return new ActiveSubmissionState();
    }

    public ISubmissionState MoveToDeactivated()
    {
        const string message = "Submission is already completed and cannot be deactivated";
        throw new DomainInvalidOperationException(message);
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
        const string message = "Submission is already completed and cannot be completed again";
        throw new DomainInvalidOperationException(message);
    }

    public ISubmissionState MoveToReviewed()
    {
        const string message = "Submission is already completed and cannot be reviewed again";
        throw new DomainInvalidOperationException(message);
    }
}