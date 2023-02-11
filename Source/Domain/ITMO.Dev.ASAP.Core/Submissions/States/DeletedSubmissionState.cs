using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Core.Models;
using ITMO.Dev.ASAP.Core.Tools;
using ITMO.Dev.ASAP.Core.ValueObject;

namespace ITMO.Dev.ASAP.Core.Submissions.States;

public class DeletedSubmissionState : ISubmissionState
{
    public SubmissionStateKind Kind => SubmissionStateKind.Deleted;

    public bool IsTerminalEffectiveState => false;

    public ISubmissionState MoveToRated(Fraction? rating, Points? extraPoints)
    {
        string message = "Cannot update submission points. Submission state: is deleted";
        throw new DomainInvalidOperationException(message);
    }

    public ISubmissionState MoveToPointsUpdated(Fraction? rating, Points? extraPoints)
    {
        string message = "Cannot update submission points of deleted submission";
        throw new DomainInvalidOperationException(message);
    }

    public ISubmissionState MoveToBanned()
    {
        return new BannedSubmissionState();
    }

    public ISubmissionState MoveToActivated()
    {
        const string message = "Cannot activate deleted submission";
        throw new DomainInvalidOperationException(message);
    }

    public ISubmissionState MoveToDeactivated()
    {
        const string message = "Cannot deactivate deleted submission";
        throw new DomainInvalidOperationException(message);
    }

    public ISubmissionState MoveToDateUpdated(SpbDateTime newDate)
    {
        const string message = "Cannot update submission date of deleted submission";
        throw new DomainInvalidOperationException(message);
    }

    public ISubmissionState MoveToDeleted()
    {
        const string message = "Submission is already deleted";
        throw new DomainInvalidOperationException(message);
    }

    public ISubmissionState MoveToCompleted()
    {
        const string message = "Cannot complete deleted submission";
        throw new DomainInvalidOperationException(message);
    }

    public ISubmissionState MoveToReviewed()
    {
        const string message = "Cannot review deleted submission";
        throw new DomainInvalidOperationException(message);
    }
}