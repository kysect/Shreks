using Kysect.Shreks.Core.Specifications;

namespace Kysect.Shreks.Core.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<TDestination> WithSpecification<TSource, TDestination>(
        this IQueryable<TSource> query,
        ISpecification<TSource, TDestination> specification)
    {
        return specification.Apply(query);
    }
}