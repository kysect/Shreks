using Bogus;
using Microsoft.Extensions.DependencyInjection;

namespace Kysect.Shreks.Seeding.Options;

public class EntityGenerationOptions
{
    private readonly IServiceCollection _collection;

    public Faker Faker { get; set; }

    public EntityGenerationOptions(IServiceCollection collection)
    {
        _collection = collection;
        
        Faker = new Faker();
    }

    public void ConfigureEntityGenerator<TEntity>(Action<EntityGeneratorOptions<TEntity>> action)
    {
        var instance = new EntityGeneratorOptions<TEntity>();
        action(instance);
        
        _collection.AddSingleton(instance);
    }
}