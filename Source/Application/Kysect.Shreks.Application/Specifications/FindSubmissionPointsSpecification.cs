using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Core.ValueObject;

namespace Kysect.Shreks.Application.Specifications;

public static class FindSubmissionPointsSpecification
{
    public static Points? CalculateTotalSubmissionPoints(this Submission submission, DateOnly deadline)
    {
        ArgumentNullException.ThrowIfNull(submission);

        if (submission.Points is null)
            return null;

        var points = submission.Points.Value;

        var deadlinePolicy = submission.FindActiveDeadlinePolicy(deadline);

        if (deadlinePolicy is not null)
            points = deadlinePolicy.Apply(points);

        if (submission.ExtraPoints is not null)
            points += submission.ExtraPoints.Value;

        return points;
    }

    public static Points? CalculatePenaltySubmissionPoints(this Submission submission, DateOnly deadline)
    {
        ArgumentNullException.ThrowIfNull(submission);

        Points? deadlineAppliedPoints = submission.CalculateTotalSubmissionPoints(deadline);

        if (submission.Points is null)
            return null;

        if (deadlineAppliedPoints is null)
            return null;
        
        Points? penaltyPoints = submission.Points - deadlineAppliedPoints;

        return penaltyPoints;
    }
}