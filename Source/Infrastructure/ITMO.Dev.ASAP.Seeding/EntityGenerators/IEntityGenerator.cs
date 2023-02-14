namespace ITMO.Dev.ASAP.Seeding.EntityGenerators;

public interface IEntityGenerator<TEntity>
{
    IReadOnlyList<TEntity> GeneratedEntities { get; }

    TEntity Generate();
}