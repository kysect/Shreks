using ITMO.Dev.ASAP.Application.Dto.Querying;

namespace ITMO.Dev.ASAP.Application.Queries;

public record struct EntityQueryRequest<TEntity, TParameter>(
    IQueryable<TEntity> Query,
    QueryParameter<TParameter> Parameter);