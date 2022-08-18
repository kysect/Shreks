using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Application.Handlers.Extensions;
using Kysect.Shreks.Core.Formatters;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Context;
using Kysect.Shreks.DataAccess.Extensions;
using Kysect.Shreks.Integration.Google;
using Kysect.Shreks.Integration.Google.Extensions;
using Kysect.Shreks.Integration.Google.Providers;
using Kysect.Shreks.Seeding.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

var credential = await GoogleCredential.FromFileAsync("client_secrets.json", default);

var initializer = new BaseClientService.Initializer
{
    HttpClientInitializer = credential
};

var sheetsService = new SheetsService(initializer);

var driverService = new DriveService(initializer);
//TODO: insert valid credentials

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

IServiceProvider services = new ServiceCollection()
    .AddGoogleIntegration()
    .AddSingleton(sheetsService)
    .AddSingleton(driverService)
    .AddSingleton<IUserFullNameFormatter, UserFullNameFormatter>()
    .AddSingleton<ICultureInfoProvider, RuCultureInfoProvider>()
    .AddEntityGenerators(o => o
        .ConfigureEntityGenerator<Submission>(s => s.Count = 1000)
        .ConfigureEntityGenerator<Student>(s => s.Count = 600)
        .ConfigureEntityGenerator<StudentGroup>(s => s.Count = 50)
        .ConfigureEntityGenerator<SubjectCourseGroup>(s => s.Count = 50)
        .ConfigureEntityGenerator<Assignment>(a => a.Count = 50)
        .ConfigureFaker(f => f.Locale = "ru"))
    .AddDatabaseContext(o => o
        .UseLazyLoadingProxies()
        .UseInMemoryDatabase("Data Source=playground.db"))
    .AddDatabaseSeeders()
    .AddHandlers()
    .AddLogging(o => o.AddSerilog())
    .BuildServiceProvider();

var databaseContext = services.GetRequiredService<ShreksDatabaseContext>();
databaseContext.Database.EnsureCreated();
await databaseContext.SaveChangesAsync();
await services.UseDatabaseSeeders();

var subjectCourse = databaseContext.SubjectCourses.First();

var googleTableAccessor = services.GetRequiredService<IGoogleTableAccessor>();

await ((GoogleTableAccessorWorker)googleTableAccessor).StartAsync(default);

await googleTableAccessor.UpdatePointsAsync(subjectCourse.Id);
await googleTableAccessor.UpdateQueueAsync(subjectCourse.Id);

await Task.Delay(TimeSpan.FromSeconds(30));

var anotherSubjectCourse = databaseContext.SubjectCourses.Skip(1).First();
await googleTableAccessor.UpdatePointsAsync(anotherSubjectCourse.Id);
await googleTableAccessor.UpdateQueueAsync(anotherSubjectCourse.Id);

await Task.Delay(TimeSpan.FromMinutes(2));

await googleTableAccessor.UpdatePointsAsync(subjectCourse.Id);
await googleTableAccessor.UpdateQueueAsync(subjectCourse.Id);
await googleTableAccessor.UpdatePointsAsync(subjectCourse.Id);
await googleTableAccessor.UpdateQueueAsync(subjectCourse.Id);

await Task.Delay(-1);