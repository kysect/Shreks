using Kysect.Shreks.Application.Abstractions.Exceptions;
using Microsoft.EntityFrameworkCore;
using RichEntity.Annotations;

namespace Kysect.Shreks.DataAccess.Abstractions.Extensions;

public static class DbSetExtensions
{
    public static async Task<T> GetByIdAsync<T, TKey>(
        this DbSet<T> dbSet,
        TKey id,
        CancellationToken cancellationToken = default)
        where T : class, IEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        var entity = await dbSet.FindAsync(new object[] { id }, cancellationToken);

        if (entity is null)
            throw new EntityNotFoundException($"Entity of type {typeof(T).Name} with id {id} not found");

        return entity;
    }
}