using ITMO.Dev.ASAP.Core.Queue;
using Microsoft.EntityFrameworkCore;

namespace ITMO.Dev.ASAP.Application.Tools;

internal class QueryExecutor : IQueryExecutor
{
    public async Task<IReadOnlyCollection<T>> ExecuteAsync<T>(IQueryable<T> query, CancellationToken cancellationToken)
    {
        return await query.ToListAsync(cancellationToken);
    }
}