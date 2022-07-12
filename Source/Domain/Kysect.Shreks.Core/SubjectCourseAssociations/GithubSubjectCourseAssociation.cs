using Kysect.Shreks.Core.Study;

namespace Kysect.Shreks.Core.SubjectCourseAssociations;

public partial class GithubSubjectCourseAssociation : SubjectCourseAssociation
{
    public GithubSubjectCourseAssociation(SubjectCourse subjectCourse, string githubOrganizationName) : base(subjectCourse)
    {
        GithubOrganizationName = githubOrganizationName;
    }

    public string GithubOrganizationName { get; protected set; }
}