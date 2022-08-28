using Kysect.Shreks.Core.Queue;
using Microsoft.EntityFrameworkCore;

namespace Kysect.Shreks.Application.Tools;

internal class QueryExecutor : IQueryExecutor
{
    public async Task<IReadOnlyCollection<T>> ExecuteAsync<T>(IQueryable<T> query, CancellationToken cancellationToken)
        => await query.ToListAsync(cancellationToken);
}