namespace Kysect.Shreks.Application.Dto.Study;

public record AssignmentDto(
    Guid SubjectCourseId,
    Guid Id,
    string Title,
    string ShortName,
    int Order,
    double MinPoints,
    double MaxPoints);