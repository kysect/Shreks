using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Context;
using Kysect.Shreks.DataAccess.Extensions;
using Kysect.Shreks.Seeding.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Kysect.Shreks.Tests.DataAccess;

public abstract class DataAccessTestBase : IDisposable
{
    protected readonly ShreksDatabaseContext Context;
    protected readonly IServiceProvider Provider;

    protected DataAccessTestBase()
    {
        var collection = new ServiceCollection();
        var id = Guid.NewGuid();

        collection.AddDatabaseContext(x => { x.UseLazyLoadingProxies().UseSqlite($"Data Source={id}.db"); });

        collection.AddEntityGenerators(x =>
        {
            x.ConfigureEntityGenerator<StudentGroup>(xx => { xx.Count = 10; });

            x.ConfigureEntityGenerator<Student>(xx => { xx.Count = 30; });
        });

        Provider = collection.BuildServiceProvider();

        Context = Provider.GetRequiredService<ShreksDatabaseContext>();
        Context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        Context.Dispose();

        GC.SuppressFinalize(this);
    }
}