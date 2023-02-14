using Bogus;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.Core.SubjectCourseAssociations;
using ITMO.Dev.ASAP.Core.Submissions;
using ITMO.Dev.ASAP.Core.UserAssociations;
using ITMO.Dev.ASAP.Core.Users;
using ITMO.Dev.ASAP.DataAccess.Context;
using ITMO.Dev.ASAP.DataAccess.Extensions;
using ITMO.Dev.ASAP.Seeding.Extensions;
using ITMO.Dev.ASAP.Seeding.Options;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ITMO.Dev.ASAP.Tests;

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
        collection.AddMediatR(typeof(TestBase));

        // TODO: Do not call virtual methods in constructor
#pragma warning disable CA2214

        // ReSharper disable once VirtualMemberCallInConstructor
        ConfigureServices(collection);
#pragma warning restore CA2214

        Provider = collection.BuildServiceProvider();

        Context = Provider.GetRequiredService<DatabaseContext>();
        Context.Database.EnsureCreated();

        Provider.UseDatabaseSeeders().GetAwaiter().GetResult();
    }

    protected DatabaseContext Context { get; }

    protected IServiceProvider Provider { get; }

    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        Context.Dispose();
    }

    protected virtual void ConfigureServices(IServiceCollection collection) { }

    protected virtual void ConfigureSeeding(EntityGenerationOptions options) { }
}