using ITMO.Dev.ASAP.Core.SubjectCourseAssociations;
using ITMO.Dev.ASAP.Core.Users;

namespace ITMO.Dev.ASAP.Core.Specifications.Github;

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