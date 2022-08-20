using AutoMapper;
using Kysect.Shreks.DataAccess.Context;
using Kysect.Shreks.DataAccess.Extensions;
using Kysect.Shreks.Mapping.Extensions;
using Kysect.Shreks.Seeding.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Kysect.Shreks.Tests.Application;

public class ApplicationTestBase : IDisposable
{
    protected readonly ShreksDatabaseContext Context;
    protected readonly IMapper Mapper;
    protected readonly IServiceProvider Provider;

    protected ApplicationTestBase()
    {
        var collection = new ServiceCollection();
        var id = Guid.NewGuid();

        collection.AddDatabaseContext(x =>
        {
            x.UseLazyLoadingProxies().UseSqlite($"Data Source={id}.db");
        });

        collection.AddEntityGenerators();
        collection.AddDatabaseSeeders();
        collection.AddMappingConfiguration();

        Provider = collection.BuildServiceProvider();

        Context = Provider.GetRequiredService<ShreksDatabaseContext>();
        Context.Database.EnsureCreated();

        Mapper = Provider.GetRequiredService<IMapper>();

        Provider.UseDatabaseSeeders().GetAwaiter().GetResult();
    }

    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        Context.Dispose();

        GC.SuppressFinalize(this);
    }
}