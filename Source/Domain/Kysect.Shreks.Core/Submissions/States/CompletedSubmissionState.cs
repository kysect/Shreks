using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.Models;
using Kysect.Shreks.Core.Tools;
using Kysect.Shreks.Core.ValueObject;

namespace Kysect.Shreks.Core.Submissions.States;

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
        => this;

    public ISubmissionState MoveToBanned()
        => new BannedSubmissionState();

    public ISubmissionState MoveToActivated()
        => new ActiveSubmissionState();

    public ISubmissionState MoveToDeactivated()
    {
        const string message = "Submission is already completed and cannot be deactivated";
        throw new DomainInvalidOperationException(message);
    }

    public ISubmissionState MoveToDateUpdated(SpbDateTime newDate)
        => this;

    public ISubmissionState MoveToDeleted()
        => new DeletedSubmissionState();

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