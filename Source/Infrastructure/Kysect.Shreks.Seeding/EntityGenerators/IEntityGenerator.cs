namespace Kysect.Shreks.Seeding.EntityGenerators;

public interface IEntityGenerator<TEntity>
{
    IReadOnlyList<TEntity> GeneratedEntities { get; }

    TEntity Generate();
}