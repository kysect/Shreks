namespace Kysect.Shreks.Application.Dto.SubjectCourseAssociations;

public record GithubSubjectCourseAssociationDto(string GithubOrganizationName, string TemplateRepositoryName)
    : SubjectCourseAssociationDto;