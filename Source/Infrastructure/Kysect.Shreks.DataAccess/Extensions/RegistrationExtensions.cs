using Kysect.Shreks.Application.Abstractions.Tools;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Context;
using Kysect.Shreks.DataAccess.Tools;
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
        collection.AddSingleton(typeof(IPatternMatcher<>), typeof(PatternMatcher<>));

        return collection;
    }

    public static Task UseDatabaseContext(this IServiceProvider provider)
    {
        ShreksDatabaseContext context = provider.GetRequiredService<ShreksDatabaseContext>();
        return context.Database.MigrateAsync();
    }
}