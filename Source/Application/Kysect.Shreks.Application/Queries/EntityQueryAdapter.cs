using FluentChaining;
using Kysect.Shreks.Application.Dto.Querying;

namespace Kysect.Shreks.Application.Queries;

public class EntityQueryAdapter<TEntity, TParameter> : IEntityQuery<TEntity, TParameter>
{
    private readonly IChain<EntityQueryRequest<TEntity, TParameter>, IQueryable<TEntity>> _chain;

    public EntityQueryAdapter(IChain<EntityQueryRequest<TEntity, TParameter>, IQueryable<TEntity>> chain)
    {
        _chain = chain;
    }

    public IQueryable<TEntity> Apply(IQueryable<TEntity> query, QueryConfiguration<TParameter> configuration)
    {
        foreach (QueryParameter<TParameter> parameter in configuration.Parameters)
        {
            var request = new EntityQueryRequest<TEntity, TParameter>(query, parameter);
            query = _chain.Process(request);
        }

        return query;
    }
}