using Google.Apis.Auth.OAuth2;
using Kysect.Shreks.Application.Abstractions.Formatters;
using Kysect.Shreks.Application.Handlers.Extensions;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Context;
using Kysect.Shreks.DataAccess.Extensions;
using Kysect.Shreks.Integration.Google;
using Kysect.Shreks.Integration.Google.Extensions;
using Kysect.Shreks.Integration.Google.Providers;
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
    .AddSingleton<IUserFullNameFormatter, UserFullNameFormatter>()
    .AddSingleton<ICultureInfoProvider, RuCultureInfoProvider>()
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
    .BuildServiceProvider();

var databaseContext = services.GetRequiredService<ShreksDatabaseContext>();
databaseContext.Database.EnsureCreated();
await databaseContext.SaveChangesAsync();
await services.UseDatabaseSeeders();

var subjectCourse = databaseContext.SubjectCourses.First();

var tableQueue = services.GetRequiredService<TableUpdateQueue>();
var tableWorker = services.GetRequiredService<GoogleTableUpdateWorker>();

await tableWorker.StartAsync(default);

tableQueue.EnqueueCoursePointsUpdate(subjectCourse.Id);
tableQueue.EnqueueSubmissionsQueueUpdate(subjectCourse.Id);

await Task.Delay(TimeSpan.FromSeconds(30));

var anotherSubjectCourse = databaseContext.SubjectCourses.Skip(1).First();
tableQueue.EnqueueCoursePointsUpdate(anotherSubjectCourse.Id);
tableQueue.EnqueueSubmissionsQueueUpdate(anotherSubjectCourse.Id);

await Task.Delay(TimeSpan.FromMinutes(2));

tableQueue.EnqueueCoursePointsUpdate(subjectCourse.Id);
tableQueue.EnqueueSubmissionsQueueUpdate(subjectCourse.Id);
tableQueue.EnqueueCoursePointsUpdate(subjectCourse.Id);
tableQueue.EnqueueSubmissionsQueueUpdate(subjectCourse.Id);

await Task.Delay(-1);