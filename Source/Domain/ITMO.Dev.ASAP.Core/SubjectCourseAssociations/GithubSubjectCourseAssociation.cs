using ITMO.Dev.ASAP.Core.Study;

namespace ITMO.Dev.ASAP.Core.SubjectCourseAssociations;

public partial class GithubSubjectCourseAssociation : SubjectCourseAssociation
{
    public GithubSubjectCourseAssociation(
        Guid id,
        SubjectCourse subjectCourse,
        string githubOrganizationName,
        string templateRepositoryName,
        string mentorTeamName)
        : base(id, subjectCourse)
    {
        GithubOrganizationName = githubOrganizationName;
        TemplateRepositoryName = templateRepositoryName;
        MentorTeamName = mentorTeamName;
    }

    public string GithubOrganizationName { get; protected set; }

    public string TemplateRepositoryName { get; protected set; }

    public string MentorTeamName { get; set; }
}