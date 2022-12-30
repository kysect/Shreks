namespace Kysect.Shreks.WebApi.Abstractions.Models.GroupAssignments;

public record CreateGroupAssignmentRequest(Guid GroupId, Guid AssignmentId, DateTime Deadline);