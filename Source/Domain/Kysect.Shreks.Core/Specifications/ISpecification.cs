namespace Kysect.Shreks.Core.Specifications;

public interface ISpecification<in TSource, out TDestination>
{
    IQueryable<TDestination> Apply(IQueryable<TSource> query);
}