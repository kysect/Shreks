using System.Text;

namespace ITMO.Dev.ASAP.Application.Dto.Study;

public record SubmissionDto(
    Guid Id,
    int Code,
    DateTime SubmissionDate,
    Guid StudentId,
    Guid AssignmentId,
    string Payload,
    double? ExtraPoints,
    double? Points,
    string AssignmentShortName,
    SubmissionStateDto State)
{
    public string ToDisplayString()
    {
        var stringBuilder = new StringBuilder();

        stringBuilder
            .AppendLine($"Submission code: {Code}")
            .AppendLine($"- Submitted: {SubmissionDate.ToString("dd.MM.yyyy")}");

        if (Points.HasValue)
            stringBuilder.AppendLine($"- Point: {Points}");

        if (ExtraPoints.HasValue)
            stringBuilder.AppendLine($"- Extra points: {ExtraPoints}");

        return stringBuilder.ToString();
    }
}