namespace ITMO.Dev.ASAP.Application.Dto.Tables;

public record struct SubmissionsQueueDto(string GroupName, IReadOnlyList<QueueSubmissionDto> Submissions);