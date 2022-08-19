namespace Kysect.Shreks.Core.Queue;

public interface IQueryExecutor
{
    Task<IReadOnlyCollection<T>> ExecuteAsync<T>(IQueryable<T> query, CancellationToken cancellationToken);
}