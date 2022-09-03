using System.Text;

namespace Kysect.Shreks.Application.Dto.Study;

public record SubmissionRateDto(
    int Code,
    DateTime SubmissionDate,
    double? Rating,
    double? RawPoints,
    double? MaxRawPoints,
    double? ExtraPoints,
    double? PenaltyPoints,
    double? TotalPoints)
{
    public string ToPullRequestString()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine($"Submission code: {Code} ({SubmissionDate})");

        // TODO: replace with Rating type?
        if (Rating is not null)
            stringBuilder.AppendLine($"Rating: {Rating * 100}");

        if (RawPoints is not null)
        {
            if (ExtraPoints is not null && ExtraPoints != 0)
                stringBuilder.AppendLine($"Points: {RawPoints}/{MaxRawPoints} (+{ExtraPoints} extra points)");
            else
                stringBuilder.AppendLine($"Points: {RawPoints}/{MaxRawPoints}");
        }

        if (PenaltyPoints is not null)
            stringBuilder.AppendLine($"Penalty points: {PenaltyPoints}");

        if (TotalPoints is not null)
            stringBuilder.AppendLine($"Total points: {TotalPoints}");

        return stringBuilder.ToString();
    }
}