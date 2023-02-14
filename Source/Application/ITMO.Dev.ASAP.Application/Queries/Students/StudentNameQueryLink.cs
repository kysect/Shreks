using ITMO.Dev.ASAP.Application.Abstractions.Tools;
using ITMO.Dev.ASAP.Application.Dto.Querying;
using ITMO.Dev.ASAP.Application.Extensions;
using ITMO.Dev.ASAP.Core.Users;

namespace ITMO.Dev.ASAP.Application.Queries.Students;

public class StudentNameQueryLink : QueryLinkBase<Student, StudentQueryParameter>
{
    private readonly IPatternMatcher<Student> _matcher;

    public StudentNameQueryLink(IPatternMatcher<Student> matcher)
    {
        _matcher = matcher;
    }

    protected override IQueryable<Student>? TryApply(
        IQueryable<Student> query,
        QueryParameter<StudentQueryParameter> parameter)
    {
        if (parameter.Type is not StudentQueryParameter.Name)
            return null;

        return query.Where(
            _matcher.Match(x => x.User.FirstName, parameter.Pattern)
                .Or(_matcher.Match(x => x.User.LastName, parameter.Pattern))
                .Or(_matcher.Match(x => x.User.MiddleName, parameter.Pattern)));
    }
}