using Kysect.Shreks.DataAccess.Context;
using Kysect.Shreks.DataAccess.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Kysect.Shreks.Tests.DataAccess;

public abstract class DataAccessTestBase : IDisposable
{
    protected readonly ShreksDatabaseContext Context;

    protected DataAccessTestBase()
    {
        var collection = new ServiceCollection();
        collection.AddDatabaseContext(x =>
        {
            x.UseLazyLoadingProxies().UseSqlite("Data Source=test.db");
        });

        var provider = collection.BuildServiceProvider();
        
        Context = provider.GetRequiredService<ShreksDatabaseContext>();
        Context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        Context.Dispose();

        GC.SuppressFinalize(this);
    }
}