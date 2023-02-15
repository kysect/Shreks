namespace ITMO.Dev.ASAP.Application.Dto.Tables;

public record AssignmentPointsDto(Guid AssignmentId, DateOnly Date, bool IsBanned, double Points);