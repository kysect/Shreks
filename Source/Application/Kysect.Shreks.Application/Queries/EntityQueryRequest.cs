using Kysect.Shreks.Application.Dto.Querying;

namespace Kysect.Shreks.Application.Queries;

public record struct EntityQueryRequest<TEntity, TParameter>(
    IQueryable<TEntity> Query,
    QueryParameter<TParameter> Parameter);