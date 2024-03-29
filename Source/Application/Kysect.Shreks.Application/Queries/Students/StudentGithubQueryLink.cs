using Kysect.Shreks.Application.Abstractions.Tools;
using Kysect.Shreks.Application.Dto.Querying;
using Kysect.Shreks.Core.UserAssociations;
using Kysect.Shreks.Core.Users;

namespace Kysect.Shreks.Application.Queries.Students;

public class StudentGithubQueryLink : QueryLinkBase<Student, StudentQueryParameter>
{
    private readonly IPatternMatcher<Student> _matcher;

    public StudentGithubQueryLink(IPatternMatcher<Student> matcher)
    {
        _matcher = matcher;
    }

    protected override IQueryable<Student>? TryApply(
        IQueryable<Student> query,
        QueryParameter<StudentQueryParameter> parameter)
    {
        if (parameter.Type is not StudentQueryParameter.GithubUsername)
            return null;

        // Possible NRE if there is no GH User Association.
#pragma warning disable CS8602
        return query.Where(_matcher.Match(
            x => x.User.Associations.OfType<GithubUserAssociation>().FirstOrDefault().GithubUsername,
            parameter.Pattern));
#pragma warning restore CS8602
    }
}