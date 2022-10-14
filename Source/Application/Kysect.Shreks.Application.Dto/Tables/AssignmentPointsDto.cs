namespace Kysect.Shreks.Application.Dto.Tables;

public record AssignmentPointsDto(Guid AssignmentId, DateOnly Date, double Points, bool IsCompromised);