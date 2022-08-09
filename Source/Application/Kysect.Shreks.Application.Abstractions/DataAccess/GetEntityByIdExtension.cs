using Kysect.Shreks.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Kysect.Shreks.Application.Abstractions.DataAccess;

public static class GetEntityByIdExtension
{
    public static async Task<T> GetEntityByIdAsync<T>(this DbSet<T> dbSet, Guid id, CancellationToken cancellationToken = default)
        where T : class
    {
        var entity = await dbSet.FindAsync(new object[]{ id }, cancellationToken);
        if (entity is null)
            throw new EntityNotFoundException($"Entity of type {typeof(T).Name} with id {id} not found");

        return entity;
    }
}