using ITMO.Dev.ASAP.Core.Study;

namespace ITMO.Dev.ASAP.Core.SubjectCourseAssociations;

public partial class GithubSubjectCourseAssociation : SubjectCourseAssociation
{
    public GithubSubjectCourseAssociation(
        Guid id,
        SubjectCourse subjectCourse,
        string githubOrganizationName,
        string templateRepositoryName)
        : base(id, subjectCourse)
    {
        GithubOrganizationName = githubOrganizationName;
        TemplateRepositoryName = templateRepositoryName;
    }

    public string GithubOrganizationName { get; protected set; }

    public string TemplateRepositoryName { get; protected set; }
}