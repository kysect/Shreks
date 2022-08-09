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
        collection.AddDatabaseContext(x =>
        {
            x.UseLazyLoadingProxies().UseSqlite($"Data Source={Guid.NewGuid()}.db");
        });

        collection.AddEntityGenerators();

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