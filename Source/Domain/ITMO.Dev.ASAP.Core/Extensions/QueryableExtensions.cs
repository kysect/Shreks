using ITMO.Dev.ASAP.Core.Specifications;

namespace ITMO.Dev.ASAP.Core.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<TDestination> WithSpecification<TSource, TDestination>(
        this IQueryable<TSource> query,
        ISpecification<TSource, TDestination> specification)
    {
        return specification.Apply(query);
    }
}