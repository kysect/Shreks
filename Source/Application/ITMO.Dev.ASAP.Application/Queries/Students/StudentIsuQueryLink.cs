using ITMO.Dev.ASAP.Application.Abstractions.Tools;
using ITMO.Dev.ASAP.Application.Dto.Querying;
using ITMO.Dev.ASAP.Core.UserAssociations;
using ITMO.Dev.ASAP.Core.Users;

namespace ITMO.Dev.ASAP.Application.Queries.Students;

public class StudentIsuQueryLink : QueryLinkBase<Student, StudentQueryParameter>
{
    private readonly IPatternMatcher<Student> _matcher;

    public StudentIsuQueryLink(IPatternMatcher<Student> matcher)
    {
        _matcher = matcher;
    }

    protected override IQueryable<Student>? TryApply(
        IQueryable<Student> query,
        QueryParameter<StudentQueryParameter> parameter)
    {
        if (parameter.Type is not StudentQueryParameter.Isu)
            return null;

        // Possible NRE if there is no Isu User Association.
#pragma warning disable CS8602
        return query.Where(
            _matcher.Match(
                x => x.User.Associations.OfType<IsuUserAssociation>().FirstOrDefault().UniversityId.ToString(),
                parameter.Pattern));
#pragma warning restore CS8602
    }
}