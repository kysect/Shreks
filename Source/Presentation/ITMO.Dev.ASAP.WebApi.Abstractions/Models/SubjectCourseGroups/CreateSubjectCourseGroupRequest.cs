namespace ITMO.Dev.ASAP.WebApi.Abstractions.Models.SubjectCourseGroups;

public record CreateSubjectCourseGroupRequest(Guid SubjectCourseId, Guid GroupId);