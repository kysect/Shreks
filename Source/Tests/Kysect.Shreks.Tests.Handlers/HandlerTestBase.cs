using AutoMapper;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.DataAccess.Context;
using Kysect.Shreks.DataAccess.Extensions;
using Kysect.Shreks.Mapping.Extensions;
using Kysect.Shreks.Seeding.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Kysect.Shreks.Tests.Handlers;

public class HandlerTestBase : IDisposable
{
    protected readonly ShreksDatabaseContext Context;
    protected readonly IMapper Mapper;
    protected readonly IServiceProvider Provider;

    protected HandlerTestBase()
    {
        var collection = new ServiceCollection();
        var id = Guid.NewGuid();

        collection.AddDatabaseContext(x =>
        {
            x.UseLazyLoadingProxies().UseSqlite($"Data Source={id}.db");
        });

        collection.AddEntityGenerators(x =>
        {
            x.ConfigureEntityGenerator<GithubSubmission>(xx => xx.Count = 500);
        });
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