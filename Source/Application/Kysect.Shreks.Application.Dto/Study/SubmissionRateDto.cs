using System.Text;

namespace Kysect.Shreks.Application.Dto.Study;

public record SubmissionRateDto(
    int Code,
    DateOnly SubmissionDate,
    double? Rating,
    double? RawPoints,
    double? ExtraPoints,
    double? PenaltyPoints,
    double? TotalPoints)
{
    public string ToPullRequestString()
    {
        return new StringBuilder()
            .AppendLine($"Submission code: {Code}")
            .AppendLine($"Rating: {Rating}")
            .AppendLine($"Raw points: {RawPoints}")
            .AppendLine($"Submitted: {SubmissionDate}")
            .AppendLine($"Penalty points: {PenaltyPoints}")
            .AppendLine($"Extra points: {ExtraPoints}")
            .AppendLine($"Total points: {TotalPoints}")
            .ToString();
    }
}