﻿using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Core.ValueObject;

namespace Kysect.Shreks.Application.Specifications;

public static class FindSubmissionPointsSpecification
{
    public static Points? GetTotalSubmissionPoints(this Submission submission, DateOnly deadline)
    {
        ArgumentNullException.ThrowIfNull(submission);

        if (submission.Points is null)
            return null;

        var points = submission.Points.Value;

        var deadlinePolicy = submission.GetActiveDeadlinePolicy(deadline);

        if (deadlinePolicy is not null)
            points = deadlinePolicy.Apply(points);

        if (submission.ExtraPoints is not null)
            points += submission.ExtraPoints.Value;

        return points;
    }

    public static Points? GetPenaltySubmissionPoints(this Submission submission, DateOnly deadline)
    {
        ArgumentNullException.ThrowIfNull(submission);

        Points? deadlineAppliedPoints = submission.GetTotalSubmissionPoints(deadline);
        double? penaltyPoints = (submission.Points - deadlineAppliedPoints)?.Value;

        return new Points(penaltyPoints!.Value);
    }
}