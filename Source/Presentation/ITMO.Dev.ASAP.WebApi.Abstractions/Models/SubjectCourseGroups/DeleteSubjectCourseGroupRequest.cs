namespace ITMO.Dev.ASAP.WebApi.Abstractions.Models.SubjectCourseGroups;

public record DeleteSubjectCourseGroupRequest(Guid SubjectCourseId, Guid GroupId);