namespace Kysect.Shreks.WebApi.Abstractions.Models.SubjectCourseGroups;

public record BulkCreateSubjectCourseGroupsRequest(Guid SubjectCourseId, IReadOnlyCollection<Guid> GroupIds);