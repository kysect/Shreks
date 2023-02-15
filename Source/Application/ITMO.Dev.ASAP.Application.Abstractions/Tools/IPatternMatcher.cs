using System.Linq.Expressions;

namespace ITMO.Dev.ASAP.Application.Abstractions.Tools;

public interface IPatternMatcher<T>
{
    Expression<Func<T, bool>> Match(Expression<Func<T, string>> extractor, string pattern);
}