using Kysect.Shreks.Application.Dto.SubjectCourseAssociations;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.SubjectCourseAssociations;

namespace Kysect.Shreks.Mapping.Mappings;

public static class SubjectCourseAssociationMapping
{
    public static SubjectCourseAssociation ToEntity(this SubjectCourseAssociationDto dto, SubjectCourse subjectCourse)
    {
        return dto switch
        {
            GithubSubjectCourseAssociationDto association => new GithubSubjectCourseAssociation(
                Guid.NewGuid(),
                subjectCourse,
                association.GithubOrganizationName,
                association.TemplateRepositoryName),

            GoogleSubjectCourseAssociationDto association => new GoogleTableSubjectCourseAssociation(
                Guid.NewGuid(),
                subjectCourse,
                association.SpreadsheetId),

            _ => throw new ArgumentOutOfRangeException(nameof(dto)),
        };
    }
}