using Kysect.Shreks.Application.Abstractions.Tools;
using Kysect.Shreks.Application.Dto.Querying;
using Kysect.Shreks.Core.Study;

namespace Kysect.Shreks.Application.Queries.Groups;

public class GroupNameQueryLink : QueryLinkBase<StudentGroup, GroupQueryParameter>
{
    private readonly IPatternMatcher<StudentGroup> _matcher;

    public GroupNameQueryLink(IPatternMatcher<StudentGroup> matcher)
    {
        _matcher = matcher;
    }

    protected override IQueryable<StudentGroup> TryApply(
        IQueryable<StudentGroup> query,
        QueryParameter<GroupQueryParameter> parameter)
    {
        if (parameter.Type is not GroupQueryParameter.Name)
            return query;

        return query.Where(_matcher.Match(x => x.Name, parameter.Pattern));
    }
}