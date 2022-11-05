using Kysect.Shreks.Application.Abstractions.Tools;
using Kysect.Shreks.Application.Dto.Querying;
using Kysect.Shreks.Application.Dto.Users;
using Kysect.Shreks.Application.Extensions;
using Kysect.Shreks.Core.Users;

namespace Kysect.Shreks.Application.Queries.Students;

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