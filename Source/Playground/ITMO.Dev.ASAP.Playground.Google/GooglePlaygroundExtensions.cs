using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.Core.Submissions;
using ITMO.Dev.ASAP.Core.Users;
using ITMO.Dev.ASAP.DataAccess.Context;
using ITMO.Dev.ASAP.DataAccess.Extensions;
using ITMO.Dev.ASAP.Seeding.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ITMO.Dev.ASAP.Playground.Google;

public static class GooglePlaygroundExtensions
{
    public static IServiceCollection AddGooglePlaygroundDatabase(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddEntityGenerators(o => o
                .ConfigureEntityGenerator<GithubSubmission>(s => s.Count = 5000)
                .ConfigureEntityGenerator<User>(s => s.Count = 500)
                .ConfigureEntityGenerator<Student>(s => s.Count = 400)
                .ConfigureEntityGenerator<StudentGroup>(s => s.Count = 20)
                .ConfigureEntityGenerator<SubjectCourseGroup>(s => s.Count = 200)
                .ConfigureEntityGenerator<Assignment>(a => a.Count = 50)
                .ConfigureEntityGenerator<GroupAssignment>(a => a.Count = 500)
                .ConfigureFaker(f => f.Locale = "ru"))
            .AddDatabaseContext(o => o
                .UseLazyLoadingProxies()
                .UseInMemoryDatabase("Data Source=playground.db"))
            .AddDatabaseSeeders();
    }

    public static async Task<IServiceProvider> EnsureDatabaseSeeded(this IServiceProvider services)
    {
        DatabaseContext databaseContext = services.GetRequiredService<DatabaseContext>();
        await databaseContext.Database.EnsureCreatedAsync();
        await databaseContext.SaveChangesAsync();
        await services.UseDatabaseSeeders();

        return services;
    }
}