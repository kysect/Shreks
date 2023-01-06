using Kysect.Shreks.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using RichEntity.Annotations;

namespace Kysect.Shreks.DataAccess.Abstractions.Extensions;

public static class DbSetExtensions
{
    public static async Task<T> GetByIdAsync<T, TKey>(
        this DbSet<T> dbSet,
        TKey id,
        CancellationToken cancellationToken = default)
        where T : class
        where TKey : IEquatable<TKey>
    {
        T? entity = await dbSet.FindAsync(new object[] { id }, cancellationToken);

        return entity ?? throw new EntityNotFoundException($"Entity of type {typeof(T).Name} with id {id} not found");
    }

    public static async Task<T> GetByIdAsync<T, TKey>(
        this IQueryable<T> dbSet,
        TKey id,
        CancellationToken cancellationToken = default)
        where T : class, IEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        T? entity = await dbSet.SingleOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);

        return entity ?? throw new EntityNotFoundException($"Entity of type {typeof(T).Name} with id {id} not found");
    }
}