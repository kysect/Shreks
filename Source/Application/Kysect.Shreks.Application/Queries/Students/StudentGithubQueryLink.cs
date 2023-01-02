using Kysect.Shreks.Application.Abstractions.Tools;
using Kysect.Shreks.Application.Dto.Querying;
using Kysect.Shreks.Application.Dto.Users;
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

        return query.Where(_matcher.Match(
#pragma warning disable CS8602
            // TODO: Possible NRE if there is no GH User Association.
            x => x.User.Associations.OfType<GithubUserAssociation>().FirstOrDefault().GithubUsername,
#pragma warning restore CS8602
            parameter.Pattern));
    }
}