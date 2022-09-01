namespace Kysect.Shreks.Application.Dto.SubjectCourseAssociations;

public record GithubSubjectCourseAssociationDto(
    Guid SubjectCourseId,
    string SubjectCourseName,
    string GithubOrganizationName,
    string TemplateRepositoryName);