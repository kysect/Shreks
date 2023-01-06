using AutoMapper;
using Bogus;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.SubjectCourseAssociations;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Core.UserAssociations;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Context;
using Kysect.Shreks.DataAccess.Extensions;
using Kysect.Shreks.Mapping.Extensions;
using Kysect.Shreks.Seeding.Extensions;
using Kysect.Shreks.Seeding.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Kysect.Shreks.Tests;

public class TestBase : IDisposable
{
    public TestBase()
    {
        var collection = new ServiceCollection();
        var id = Guid.NewGuid();

        Randomizer.Seed = new Random(101);

        collection.AddDatabaseContext(x => x.UseLazyLoadingProxies().UseSqlite($"Data Source={id}.db"));

        collection.AddEntityGenerators(x =>
        {
            x.ConfigureEntityGenerator<GithubSubmission>(xx => xx.Count = 1000);
            x.ConfigureEntityGenerator<SubjectCourse>(xx => xx.Count = 1);
            x.ConfigureEntityGenerator<GithubSubjectCourseAssociation>(xx => xx.Count = 1);

            x.ConfigureEntityGenerator<Student>(xx => xx.Count = 50);
            x.ConfigureEntityGenerator<User>(xx => xx.Count = 100);
            x.ConfigureEntityGenerator<GithubUserAssociation>(xx => xx.Count = 100);

            ConfigureSeeding(x);
        });

        collection.AddDatabaseSeeders();
        collection.AddMappingConfiguration();

        // TODO: Do not call virtual methods in constructor
#pragma warning disable CA2214

        // ReSharper disable once VirtualMemberCallInConstructor
        ConfigureServices(collection);
#pragma warning restore CA2214

        Provider = collection.BuildServiceProvider();

        Context = Provider.GetRequiredService<ShreksDatabaseContext>();
        Context.Database.EnsureCreated();

        Mapper = Provider.GetRequiredService<IMapper>();

        Provider.UseDatabaseSeeders().GetAwaiter().GetResult();
    }

    protected ShreksDatabaseContext Context { get; }

    protected IMapper Mapper { get; }

    protected IServiceProvider Provider { get; }

    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        Context.Dispose();
    }

    protected virtual void ConfigureServices(IServiceCollection collection) { }

    protected virtual void ConfigureSeeding(EntityGenerationOptions options) { }
}