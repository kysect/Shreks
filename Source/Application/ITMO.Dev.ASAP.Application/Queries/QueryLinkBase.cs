using FluentChaining;
using ITMO.Dev.ASAP.Application.Dto.Querying;

namespace ITMO.Dev.ASAP.Application.Queries;

public abstract class QueryLinkBase<TEntity, TParameter> :
    ILink<EntityQueryRequest<TEntity, TParameter>, IQueryable<TEntity>>
{
    public IQueryable<TEntity> Process(
        EntityQueryRequest<TEntity, TParameter> request,
        SynchronousContext context,
        LinkDelegate<EntityQueryRequest<TEntity, TParameter>, SynchronousContext, IQueryable<TEntity>> next)
    {
        IQueryable<TEntity>? result = TryApply(request.Query, request.Parameter);
        return result ?? next.Invoke(request, context);
    }

    protected abstract IQueryable<TEntity>? TryApply(IQueryable<TEntity> query, QueryParameter<TParameter> parameter);
}