namespace ITMO.Dev.ASAP.WebApi.Abstractions.Models.SubjectCourseGroups;

public record BulkCreateSubjectCourseGroupsRequest(Guid SubjectCourseId, IReadOnlyCollection<Guid> GroupIds);