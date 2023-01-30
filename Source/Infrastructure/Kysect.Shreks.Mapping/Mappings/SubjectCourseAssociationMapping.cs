using Kysect.Shreks.Application.Dto.SubjectCourseAssociations;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.SubjectCourseAssociations;

namespace Kysect.Shreks.Mapping.Mappings;

public static class SubjectCourseAssociationMapping
{
    public static GithubSubjectCourseAssociationDto ToDto(this GithubSubjectCourseAssociation association)
        => new GithubSubjectCourseAssociationDto(association.GithubOrganizationName, association.TemplateRepositoryName);

    public static GoogleSubjectCourseAssociationDto ToDto(this GoogleTableSubjectCourseAssociation association)
        => new GoogleSubjectCourseAssociationDto(association.SpreadsheetId);

    public static SubjectCourseAssociationDto ToDto(this SubjectCourseAssociation entity)
    {
        return entity switch
        {
            GithubSubjectCourseAssociation association => association.ToDto(),
            GoogleTableSubjectCourseAssociation association => association.ToDto(),
            _ => throw new ArgumentOutOfRangeException(nameof(entity)),
        };
    }

    public static GithubSubjectCourseAssociation ToEntity(
        this GithubSubjectCourseAssociationDto dto,
        SubjectCourse subjectCourse)
    {
        return new GithubSubjectCourseAssociation(
            Guid.NewGuid(),
            subjectCourse,
            dto.GithubOrganizationName,
            dto.TemplateRepositoryName);
    }

    public static GoogleTableSubjectCourseAssociation ToEntity(
        this GoogleSubjectCourseAssociationDto dto,
        SubjectCourse subjectCourse)
    {
        return new GoogleTableSubjectCourseAssociation(
            Guid.NewGuid(),
            subjectCourse,
            dto.SpreadsheetId);
    }

    public static SubjectCourseAssociation ToEntity(this SubjectCourseAssociationDto dto, SubjectCourse subjectCourse)
    {
        return dto switch
        {
            GithubSubjectCourseAssociationDto association => association.ToEntity(subjectCourse),
            GoogleSubjectCourseAssociationDto association => association.ToEntity(subjectCourse),
            _ => throw new ArgumentOutOfRangeException(nameof(dto)),
        };
    }
}