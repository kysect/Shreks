using Kysect.Shreks.Application.Dto.Querying;

namespace Kysect.Shreks.Application.Queries;

public interface IEntityQuery<TEntity, TParameter>
{
    IQueryable<TEntity> Apply(IQueryable<TEntity> query, QueryConfiguration<TParameter> configuration);
}