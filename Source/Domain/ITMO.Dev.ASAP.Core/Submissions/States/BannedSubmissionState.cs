using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Core.Models;
using ITMO.Dev.ASAP.Core.Tools;
using ITMO.Dev.ASAP.Core.ValueObject;

namespace ITMO.Dev.ASAP.Core.Submissions.States;

public class BannedSubmissionState : ISubmissionState
{
    public SubmissionStateKind Kind => SubmissionStateKind.Banned;

    public bool IsTerminalEffectiveState => true;

    public ISubmissionState MoveToRated(Fraction? rating, Points? extraPoints)
    {
        string message = $"Submission {this} is banned and cannot be rated";
        throw new DomainInvalidOperationException(message);
    }

    public ISubmissionState MoveToPointsUpdated(Fraction? rating, Points? extraPoints)
    {
        const string message = "Cannot update points of banned submission";
        throw new DomainInvalidOperationException(message);
    }

    public ISubmissionState MoveToBanned()
    {
        string message = $"Submission {this} is already banned";
        throw new DomainInvalidOperationException(message);
    }

    public ISubmissionState MoveToActivated()
    {
        const string message = "Cannot activate banned submission";
        throw new DomainInvalidOperationException(message);
    }

    public ISubmissionState MoveToDeactivated()
    {
        const string message = "Cannot deactivate banned submission";
        throw new DomainInvalidOperationException(message);
    }

    public ISubmissionState MoveToDateUpdated(SpbDateTime newDate)
    {
        const string message = "Cannot update date of banned submission";
        throw new DomainInvalidOperationException(message);
    }

    public ISubmissionState MoveToDeleted()
    {
        const string message = "Cannot delete banned submission";
        throw new DomainInvalidOperationException(message);
    }

    public ISubmissionState MoveToCompleted()
    {
        const string message = "Cannot complete banned submission";
        throw new DomainInvalidOperationException(message);
    }

    public ISubmissionState MoveToReviewed()
    {
        const string message = "Cannot review banned submission";
        throw new DomainInvalidOperationException(message);
    }
}