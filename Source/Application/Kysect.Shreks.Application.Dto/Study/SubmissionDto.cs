namespace Kysect.Shreks.Application.Dto.Study;

public record SubmissionDto(
    Guid Id,
    DateTime SubmissionDateTime,
    Guid StudentId,
    Guid AssignmentId,
    string Payload,
    double ExtraPoints,
    double Points);