namespace Kysect.Shreks.Application.Dto.Tables;

public record struct SubmissionsQueueDto(string GroupName, IReadOnlyCollection<QueueSubmissionDto> Submissions);