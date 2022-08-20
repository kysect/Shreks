using Kysect.Shreks.DataAccess.Abstractions;

namespace Kysect.Shreks.Seeding.DatabaseSeeders;

public interface IDatabaseSeeder
{
    int Priority => 0;
    
    void Seed(IShreksDatabaseContext context);
}