using System.Text;

namespace Kysect.Shreks.Application.Dto.Study;

public record SubmissionRateDto(
    int Code,
    DateTime SubmissionDate,
    double? Rating,
    double? RawPoints,
    double? ExtraPoints,
    double? PenaltyPoints,
    double? TotalPoints)
{
    public string ToPullRequestString()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine($"Submission code: {Code}");
        stringBuilder.AppendLine($"Rating: {SubmissionDate}");

        if (Rating is not null)
            stringBuilder.AppendLine($"Rating: {Rating}");

        if (RawPoints is not null)
            stringBuilder.AppendLine($"Rating: {RawPoints}");

        if (PenaltyPoints is not null)
            stringBuilder.AppendLine($"Rating: {PenaltyPoints}");

        if (ExtraPoints is not null)
            stringBuilder.AppendLine($"Rating: {ExtraPoints}");

        if (TotalPoints is not null)
            stringBuilder.AppendLine($"Rating: {TotalPoints}");

        return stringBuilder.ToString();
    }
}