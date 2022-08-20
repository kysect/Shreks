using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Kysect.Shreks.DataAccess.Extensions;

public static class RegistrationExtensions
{
    public static IServiceCollection AddDatabaseContext(
        this IServiceCollection collection,
        Action<DbContextOptionsBuilder> action)
    {
        collection.AddDbContext<IShreksDatabaseContext, ShreksDatabaseContext>(action);

        return collection;
    }
}