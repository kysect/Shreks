using System.Linq.Expressions;

namespace ITMO.Dev.ASAP.Application.Extensions;

public static class ExpressionExtensions
{
    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
    {
        const string parameterName = "x";
        ParameterExpression parameter = Expression.Parameter(typeof(T), parameterName);

        InvocationExpression leftInvoke = Expression.Invoke(left, parameter);
        InvocationExpression rightInvoke = Expression.Invoke(right, parameter);

        BinaryExpression orExpression = Expression.Or(leftInvoke, rightInvoke);

        return (Expression<Func<T, bool>>)Expression.Lambda(orExpression, parameter);
    }
}