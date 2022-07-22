using Kysect.Shreks.Seeding.Options;

namespace Kysect.Shreks.Seeding.EntityGenerators;

public abstract class EntityGeneratorBase<TEntity> : IEntityGenerator<TEntity>
{
    private readonly Lazy<IReadOnlyList<TEntity>> _lazyGeneratedEntities;

    protected EntityGeneratorBase(EntityGeneratorOptions<TEntity> options)
    {
        _lazyGeneratedEntities = new Lazy<IReadOnlyList<TEntity>>(() =>
        {
            return Enumerable.Range(0, options.Count)
                .Select(Generate)
                .ToArray();
        });
    }

    public IReadOnlyList<TEntity> GeneratedEntities => _lazyGeneratedEntities.Value;

    public TEntity Generate()
        => Generate(0);

    protected abstract TEntity Generate(int index);
}