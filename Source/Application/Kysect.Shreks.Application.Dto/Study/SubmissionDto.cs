namespace Kysect.Shreks.Application.Dto.Study;

public record SubmissionDto(
    Guid Id,
    DateOnly SubmissionDate,
    Guid StudentId,
    Guid AssignmentId,
    string Payload,
    double? ExtraPoints,
    double? Points,
    string AssignmentShortName);