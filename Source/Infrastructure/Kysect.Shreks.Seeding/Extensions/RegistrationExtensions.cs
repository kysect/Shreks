using Bogus;
using FluentScanning;
using FluentScanning.DependencyInjection;
using Kysect.Shreks.Application.Abstractions.DataAccess;
using Kysect.Shreks.Seeding.DatabaseSeeders;
using Kysect.Shreks.Seeding.EntityGenerators;
using Kysect.Shreks.Seeding.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kysect.Shreks.Seeding.Extensions;

public static class RegistrationExtensions
{
    public static IServiceCollection AddEntityGenerators(
        this IServiceCollection collection,
        Action<EntityGenerationOptions>? action = null)
    {
        var generationOptions = new EntityGenerationOptions(collection);
        action?.Invoke(generationOptions);

        collection.AddSingleton(typeof(EntityGeneratorOptions<>));
        collection.TryAddSingleton<Faker>();

        using var scanner = collection.UseAssemblyScanner(typeof(IAssemblyMarker));

        scanner.EnqueueAdditionOfTypesThat()
            .WouldBeRegisteredAsTypesConstructedFrom(typeof(IEntityGenerator<>))
            .WithSingletonLifetime()
            .AreBasedOnTypesConstructedFrom(typeof(IEntityGenerator<>))
            .AreNotAbstractClasses()
            .AreNotInterfaces();

        return collection;
    }

    public static IServiceCollection AddDatabaseSeeders(this IServiceCollection collection)
    {
        using var scanner = collection.UseAssemblyScanner(typeof(IAssemblyMarker));

        scanner.EnqueueAdditionOfTypesThat()
            .WouldBeRegisteredAs<IDatabaseSeeder>()
            .WithSingletonLifetime()
            .AreAssignableTo<IDatabaseSeeder>()
            .AreNotAbstractClasses()
            .AreNotInterfaces();

        return collection;
    }

    public static async Task UseDatabaseSeeders(
        this IServiceProvider provider,
        CancellationToken cancellationToken = default)
    {
        using var scope = provider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<IShreksDatabaseContext>();
        IEnumerable<IDatabaseSeeder> seeders = scope.ServiceProvider.GetServices<IDatabaseSeeder>();

        foreach (var seeder in seeders)
        {
            seeder.Seed(context);
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}