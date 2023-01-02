using Kysect.Shreks.Application.Abstractions.Tools;
using Kysect.Shreks.Application.Dto.Querying;
using Kysect.Shreks.Application.Dto.Users;
using Kysect.Shreks.Core.Users;

namespace Kysect.Shreks.Application.Queries.Students;

public class StudentGroupQueryLink : QueryLinkBase<Student, StudentQueryParameter>
{
    private readonly IPatternMatcher<Student> _matcher;

    public StudentGroupQueryLink(IPatternMatcher<Student> matcher)
    {
        _matcher = matcher;
    }

    protected override IQueryable<Student>? TryApply(
        IQueryable<Student> query,
        QueryParameter<StudentQueryParameter> parameter)
    {
        if (parameter.Type is not StudentQueryParameter.Group)
            return null;

#pragma warning disable CS8602
        // TODO: If student is not assigned to group when something could explode.
        return query.Where(_matcher.Match(x => x.Group.Name, parameter.Pattern));
#pragma warning restore CS8602
    }
}