namespace ITMO.Dev.ASAP.Application.Dto.SubjectCourseAssociations;

public record GithubSubjectCourseAssociationDto(
    string GithubOrganizationName,
    string TemplateRepositoryName,
    string MentorTeamName) : SubjectCourseAssociationDto;