using Bogus;
using Microsoft.Extensions.DependencyInjection;

namespace ITMO.Dev.ASAP.Seeding.Options;

public class EntityGenerationOptions
{
    private readonly IServiceCollection _collection;

    public EntityGenerationOptions(IServiceCollection collection)
    {
        _collection = collection;
    }

    public EntityGenerationOptions ConfigureEntityGenerator<TEntity>(Action<EntityGeneratorOptions<TEntity>> action)
    {
        var instance = new EntityGeneratorOptions<TEntity>();
        action(instance);

        _collection.AddSingleton(instance);
        return this;
    }

    public EntityGenerationOptions ConfigureFaker(Action<Faker> action)
    {
        var instance = new Faker();
        action(instance);

        _collection.AddSingleton(instance);
        return this;
    }
}