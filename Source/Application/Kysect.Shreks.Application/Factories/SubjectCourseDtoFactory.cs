using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.SubjectCourseAssociations;

namespace Kysect.Shreks.Application.Factories;

public class SubjectCourseDtoFactory
{
    public static SubjectCourseDto CreateFrom(SubjectCourse subjectCourse)
    {
        List<SubjectCourseAssociationDto> associations = new List<SubjectCourseAssociationDto>();

        foreach (SubjectCourseAssociation subjectCourseAssociation in subjectCourse.Associations)
        {
            switch (subjectCourseAssociation)
            {
                case GithubSubjectCourseAssociation githubSubjectCourseAssociation:
                    string githubValue = $"Repo: {githubSubjectCourseAssociation.GithubOrganizationName}, Template: {githubSubjectCourseAssociation.TemplateRepositoryName}";
                    associations.Add(new SubjectCourseAssociationDto(nameof(GithubSubjectCourseAssociation), githubValue));
                    break;
                    
                case GoogleTableSubjectCourseAssociation googleTableSubjectCourseAssociation:
                    string googleValue = $"SpreadsheetId: {googleTableSubjectCourseAssociation.SpreadsheetId}";
                    associations.Add(new SubjectCourseAssociationDto(nameof(GoogleTableSubjectCourseAssociation), googleValue));
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(subjectCourseAssociation));
            }
        }

        return new SubjectCourseDto(subjectCourse.Id, subjectCourse.Subject.Id, subjectCourse.Title, associations);
    }
}