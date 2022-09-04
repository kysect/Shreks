using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Extensions;
using Kysect.Shreks.Core.Submissions;

namespace Kysect.Shreks.Application.Factories;

public static class SubmissionRateDtoFactory
{
    public static SubmissionRateDto CreateFromSubmission(Submission submission)
    {
        ArgumentNullException.ThrowIfNull(submission);

        DateOnly deadline = submission.GroupAssignment.Deadline;

        var dto = new SubmissionRateDto
        (
            Code: submission.Code,
            SubmissionDate: submission.SubmissionDate.Value,
            Rating: submission.Rating?.Value,
            RawPoints: submission.Points?.Value,
            ExtraPoints: submission.ExtraPoints?.Value,
            PenaltyPoints: submission.CalculatePenaltySubmissionPoints(deadline)?.Value,
            TotalPoints: submission.CalculateTotalSubmissionPoints(deadline)?.Value
        );

        return dto;
    }
}