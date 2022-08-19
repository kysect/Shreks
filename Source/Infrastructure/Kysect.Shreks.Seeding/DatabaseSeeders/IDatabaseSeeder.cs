using Kysect.Shreks.Application.Abstractions.DataAccess;

namespace Kysect.Shreks.Seeding.DatabaseSeeders;

public interface IDatabaseSeeder
{
    int Priority => 0;
    
    void Seed(IShreksDatabaseContext context);
}