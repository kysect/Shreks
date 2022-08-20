namespace Kysect.Shreks.Application.Dto.Tables;

public record struct SubmissionsQueueDto(IReadOnlyCollection<QueueSubmissionDto> Submissions);