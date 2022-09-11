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

    public static Task UseDatabaseContext(this IServiceProvider provider)
    {
        var context = provider.GetRequiredService<ShreksDatabaseContext>();
        return context.Database.MigrateAsync();
    }
}