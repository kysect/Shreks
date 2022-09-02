namespace Kysect.Shreks.Application.Dto.Study;

public record GroupAssignmentDto(Guid GroupId, string GroupName, Guid AssignmentId, string AssignmentTitle, DateOnly Deadline);