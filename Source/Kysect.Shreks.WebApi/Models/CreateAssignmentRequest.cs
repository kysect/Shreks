namespace Kysect.Shreks.WebApi.Models;

public record CreateAssignmentRequest(Guid SubjectCourseId, string Title, string ShortName, int Order, double MinPoints, double MaxPoints);