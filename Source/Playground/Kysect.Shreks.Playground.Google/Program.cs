using Kysect.Shreks.Application.Handlers.Extensions;
﻿using Google.Apis.Auth.OAuth2;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Context;
using Kysect.Shreks.DataAccess.Extensions;
using Kysect.Shreks.Integration.Google;
using Kysect.Shreks.Integration.Google.Extensions;
using Kysect.Shreks.Mapping.Extensions;
using Kysect.Shreks.Seeding.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

var googleCredentials = await GoogleCredential.FromFileAsync("client_secrets.json", default);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

IServiceProvider services = new ServiceCollection()
    .AddGoogleIntegration(o => o
        .ConfigureGoogleCredentials(googleCredentials)
        .ConfigureDriveId("17CfXw__b4nnPp7VEEgWGe-N8VptaL1hP"))
    .AddEntityGenerators(o => o
        .ConfigureEntityGenerator<Submission>(s => s.Count = 1000)
        .ConfigureEntityGenerator<User>(s => s.Count = 500)
        .ConfigureEntityGenerator<Student>(s => s.Count = 400)
        .ConfigureEntityGenerator<StudentGroup>(s => s.Count = 20)
        .ConfigureEntityGenerator<SubjectCourseGroup>(s => s.Count = 50)
        .ConfigureEntityGenerator<Assignment>(a => a.Count = 50)
        .ConfigureFaker(f => f.Locale = "ru"))
    .AddDatabaseContext(o => o
        .UseLazyLoadingProxies()
        .UseInMemoryDatabase("Data Source=playground.db"))
    .AddDatabaseSeeders()
    .AddHandlers()
    .AddMappingConfiguration()
    .AddLogging(o => o.AddSerilog())
    .AddGooglePlaygroundDatabase()
    .BuildServiceProvider();

await services.EnsureDatabaseSeeded();

var databaseContext = services.GetRequiredService<ShreksDatabaseContext>();

var subjectCourse = databaseContext.SubjectCourses.First();

var tableWorker = services.GetRequiredService<GoogleTableUpdateWorker>();

await tableWorker.StartAsync(default);

tableWorker.EnqueueCoursePointsUpdate(subjectCourse.Id);
tableWorker.EnqueueSubmissionsQueueUpdate(subjectCourse.Id);

await Task.Delay(TimeSpan.FromSeconds(30));

var anotherSubjectCourse = databaseContext.SubjectCourses.Skip(1).First();
tableWorker.EnqueueCoursePointsUpdate(anotherSubjectCourse.Id);
tableWorker.EnqueueSubmissionsQueueUpdate(anotherSubjectCourse.Id);

await Task.Delay(TimeSpan.FromMinutes(2));

tableWorker.EnqueueCoursePointsUpdate(subjectCourse.Id);
tableWorker.EnqueueSubmissionsQueueUpdate(subjectCourse.Id);
tableWorker.EnqueueCoursePointsUpdate(subjectCourse.Id);
tableWorker.EnqueueSubmissionsQueueUpdate(subjectCourse.Id);

await Task.Delay(-1);

public static class GooglePlaygroundExtensions
{
    public static IServiceCollection AddGooglePlaygroundDatabase(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddEntityGenerators(o => o
                .ConfigureEntityGenerator<Submission>(s => s.Count = 1000)
                .ConfigureEntityGenerator<Student>(s => s.Count = 400)
                .ConfigureEntityGenerator<StudentGroup>(s => s.Count = 20)
                .ConfigureEntityGenerator<SubjectCourseGroup>(s => s.Count = 50)
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