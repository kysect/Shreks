namespace Kysect.Shreks.Controllers.Models;

public record CreateAssignmentRequest(Guid SubjectCourseId, string Title, string ShortName, int Order, double MinPoints, double MaxPoints);