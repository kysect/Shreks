using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Core.ValueObject;

namespace Kysect.Shreks.Application.Factories;

public static class SubmissionRateDtoFactory
{
    public static SubmissionRateDto CreateFromSubmission(Submission submission)
    {
        ArgumentNullException.ThrowIfNull(submission);

        Points maxRowPoints = submission.GroupAssignment.Assignment.MaxPoints;

        double? rating = null;
        if (submission.Rating is not null)
            rating = submission.Rating * 100;

        var dto = new SubmissionRateDto
        (
            Code: submission.Code,
            SubmissionDate: submission.SubmissionDate.Value,
            Rating: rating,
            RawPoints: submission.Points?.Value,
            MaxRawPoints: maxRowPoints.Value,
            ExtraPoints: submission.ExtraPoints?.Value,
            PenaltyPoints: submission.PointPenalty?.Value,
            TotalPoints: submission.EffectivePoints?.Value
        );

        return dto;
    }
}