using Kysect.Shreks.Core.Models;
using Kysect.Shreks.Core.Tools;
using Kysect.Shreks.Core.ValueObject;

namespace Kysect.Shreks.Core.Submissions.States;

public interface ISubmissionState
{
    SubmissionStateKind Kind { get; }

    /// <summary>
    ///     Determines whether the state is terminal, yet relevant for the system
    /// </summary>
    bool IsTerminalEffectiveState { get; }

    ISubmissionState MoveToRated(Fraction? rating, Points? extraPoints);
    ISubmissionState MoveToPointsUpdated(Fraction? rating, Points? extraPoints);
    ISubmissionState MoveToBanned();
    ISubmissionState MoveToActivated();
    ISubmissionState MoveToDeactivated();
    ISubmissionState MoveToDateUpdated(SpbDateTime newDate);
    ISubmissionState MoveToDeleted();
    ISubmissionState MoveToCompleted();
    ISubmissionState MoveToReviewed();
}