using Kysect.Shreks.Core.Queue;
using Microsoft.EntityFrameworkCore;

namespace Kysect.Shreks.Application.Queue;

public class QueryExecutor : IQueryExecutor
{
    public async Task<IReadOnlyCollection<T>> ExecuteAsync<T>(IQueryable<T> query, CancellationToken cancellationToken)
    {
        return await query.ToListAsync(cancellationToken);
    }
}