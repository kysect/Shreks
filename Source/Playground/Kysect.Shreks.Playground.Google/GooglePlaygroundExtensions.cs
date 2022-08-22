using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Context;
using Kysect.Shreks.DataAccess.Extensions;
using Kysect.Shreks.Seeding.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Kysect.Shreks.Playground.Google;

public static class GooglePlaygroundExtensions
{
    public static IServiceCollection AddGooglePlaygroundDatabase(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddEntityGenerators(o => o
                .ConfigureEntityGenerator<Submission>(s => s.Count = 2000)
                .ConfigureEntityGenerator<User>(s => s.Count = 500)
                .ConfigureEntityGenerator<Student>(s => s.Count = 400)
                .ConfigureEntityGenerator<StudentGroup>(s => s.Count = 20)
                .ConfigureEntityGenerator<SubjectCourseGroup>(s => s.Count = 200)
                .ConfigureEntityGenerator<Assignment>(a => a.Count = 50)
                .ConfigureFaker(f => f.Locale = "ru"))
            .AddDatabaseContext(o => o
                .UseLazyLoadingProxies()
                .UseInMemoryDatabase("Data Source=playground.db"))
            .AddDatabaseSeeders();
    }

    public static async Task<IServiceProvider> EnsureDatabaseSeeded(this IServiceProvider services)
    {
        var databaseContext = services.GetRequiredService<ShreksDatabaseContext>();
        await databaseContext.Database.EnsureCreatedAsync();
        await databaseContext.SaveChangesAsync();
        await services.UseDatabaseSeeders();

        return services;
    }
}