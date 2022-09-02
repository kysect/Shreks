using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.UserAssociations;

namespace Kysect.Shreks.Core.Specifications.Github;

public class GetSubjectCourseGithubUsers : ISpecification<SubjectCourseGroup, GithubUserAssociation>
{
    private readonly Guid _subjectCourseId;

    public GetSubjectCourseGithubUsers(Guid subjectCourseId)
    {
        _subjectCourseId = subjectCourseId;
    }

    public IQueryable<GithubUserAssociation> Apply(IQueryable<SubjectCourseGroup> query)
    {
        return query
            .Where(subjectCourseGroup => subjectCourseGroup.SubjectCourseId == _subjectCourseId)
            .Select(group => group.StudentGroup)
            .SelectMany(group => group.Students)
            .SelectMany(student => student.User.Associations)
            .OfType<GithubUserAssociation>();
    }
}