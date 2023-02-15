using ITMO.Dev.ASAP.Application.Dto.Querying;

namespace ITMO.Dev.ASAP.Application.Queries;

public interface IEntityQuery<TEntity, TParameter>
{
    IQueryable<TEntity> Apply(IQueryable<TEntity> query, QueryConfiguration<TParameter> configuration);
}