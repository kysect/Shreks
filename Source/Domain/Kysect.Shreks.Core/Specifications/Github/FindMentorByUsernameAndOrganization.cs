using Kysect.Shreks.Core.SubjectCourseAssociations;
using Kysect.Shreks.Core.Users;

namespace Kysect.Shreks.Core.Specifications.Github;

public class FindMentorByUsernameAndOrganization : ISpecification<SubjectCourseAssociation, Mentor>
{
    private readonly string _organization;
    private readonly Guid _userId;

    public FindMentorByUsernameAndOrganization(Guid userId, string organization)
    {
        _userId = userId;
        _organization = organization;
    }

    public IQueryable<Mentor> Apply(IQueryable<SubjectCourseAssociation> query)
    {
        return query
            .OfType<GithubSubjectCourseAssociation>()
            .Where(sca => sca.GithubOrganizationName == _organization)
            .Select(sca => sca.SubjectCourse)
            .SelectMany(sc => sc.Mentors)
            .Where(m => m.User.Id == _userId);
    }
}