using Kysect.Shreks.Application.Abstractions.Tools;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace Kysect.Shreks.DataAccess.Tools;

public class PatternMatcher<T> : IPatternMatcher<T>
{
    private readonly MethodInfo _matchingMethod;
    private readonly Expression _functionsExpression;

    public PatternMatcher()
    {
        string methodName = nameof(NpgsqlDbFunctionsExtensions.ILike);

        _matchingMethod = typeof(NpgsqlDbFunctionsExtensions)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .First(x => x.Name.Equals(methodName, StringComparison.OrdinalIgnoreCase));

        _functionsExpression = Expression.Constant(EF.Functions);
    }

    public Expression<Func<T, bool>> Match(Expression<Func<T, string>> extractor, string pattern)
    {
        ParameterExpression parameter = extractor.Parameters.Single();
        Expression patternExpression = Expression.Constant(pattern);
        Expression body = Expression.Call(_matchingMethod, _functionsExpression, extractor.Body, patternExpression);

        return (Expression<Func<T, bool>>)Expression.Lambda(body, parameter);
    }
}