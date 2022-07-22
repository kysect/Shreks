using Kysect.Shreks.Application.Abstractions.DataAccess;

namespace Kysect.Shreks.Seeding.DatabaseSeeders;

public interface IDatabaseSeeder
{
    void Seed(IShreksDatabaseContext context);
}