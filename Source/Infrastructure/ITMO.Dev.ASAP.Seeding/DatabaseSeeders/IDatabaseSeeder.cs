using ITMO.Dev.ASAP.DataAccess.Abstractions;

namespace ITMO.Dev.ASAP.Seeding.DatabaseSeeders;

public interface IDatabaseSeeder
{
    int Priority => 0;

    void Seed(IDatabaseContext context);
}