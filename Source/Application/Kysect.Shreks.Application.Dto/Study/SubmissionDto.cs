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
    string AssignmentShortName)
{
    public string ToPullRequestString()
    {
        return $"Submission code: {Code}" +
               $"\n- Point: {Points}" +
               $"\n- Submitted: {SubmissionDate}" +
               $"\n- Extra points: {ExtraPoints})";
    }
}