using Kysect.Shreks.Core.Study;

namespace Kysect.Shreks.Core.SubjectCourseAssociations;

public partial class GithubSubjectCourseAssociation : SubjectCourseAssociation
{
    public GithubSubjectCourseAssociation(SubjectCourse subjectCourse, string githubOrganizationName, string templateRepositoryName)
        : base(subjectCourse)
    {
        GithubOrganizationName = githubOrganizationName;
        TemplateRepositoryName = templateRepositoryName;
    }

    public string GithubOrganizationName { get; protected set; }
    public string TemplateRepositoryName { get; protected set; }
}