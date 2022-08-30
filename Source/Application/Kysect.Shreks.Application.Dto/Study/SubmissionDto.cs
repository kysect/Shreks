using System.Text;

namespace Kysect.Shreks.Application.Dto.Study;

public record SubmissionDto(
    Guid Id,
    int Code,
    DateOnly SubmissionDate,
    Guid StudentId,
    Guid AssignmentId,
    string Payload,
    double? ExtraPoints,
    double? Points,
    string AssignmentShortName,
    SubmissionStateDto State)
{
    public string ToPullRequestString()
    {
        var stringBuilder = new StringBuilder();

        stringBuilder
            .AppendLine($"Submission code: {Code}")
            .AppendLine($"- Submitted: {SubmissionDate}");

        if (Points.HasValue)
            stringBuilder.AppendLine($"- Point: {Points}");

        if (ExtraPoints.HasValue)
            stringBuilder.AppendLine($"- Extra points: {ExtraPoints}");


        return $"Submission code: {Code}" +
               $"\n- Point: {Points}" +
               $"\n- Submitted: {SubmissionDate}" +
               $"\n- Extra points: {ExtraPoints}";
    }
}