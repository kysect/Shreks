using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;
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

var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read);
var secrets = await GoogleClientSecrets.FromStreamAsync(stream);

var scopes = new[]
{
    SheetsService.Scope.Spreadsheets,
    DriveService.Scope.Drive
};

var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
    secrets.Secrets,
    scopes,
    "user",
    CancellationToken.None,
    new FileDataStore("token.json", true));

var initializer = new BaseClientService.Initializer
{
    HttpClientInitializer = credential
};

var sheetsService = new SheetsService(initializer);
var driverService = new DriveService(initializer);

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
    .AddLogging(o => o.AddSerilog())
    .BuildServiceProvider();

var databaseContext = services.GetRequiredService<ShreksDatabaseContext>();
databaseContext.Database.EnsureCreated();
await databaseContext.SaveChangesAsync();
await services.UseDatabaseSeeders();

var subjectCourse = databaseContext.SubjectCourses.First();

var tableWorker = services.GetRequiredService<GoogleTableUpdateWorker>();

await tableWorker.StartAsync(default);

tableWorker.EnqueueCoursePointsUpdate(subjectCourse.Id);
tableWorker.EnqueueCoursePointsUpdate(subjectCourse.Id);

await Task.Delay(TimeSpan.FromSeconds(30));

var anotherSubjectCourse = databaseContext.SubjectCourses.Skip(1).First();
tableWorker.EnqueueCoursePointsUpdate(anotherSubjectCourse.Id);
tableWorker.EnqueueCoursePointsUpdate(anotherSubjectCourse.Id);

await Task.Delay(TimeSpan.FromMinutes(2));

tableWorker.EnqueueCoursePointsUpdate(subjectCourse.Id);
tableWorker.EnqueueCoursePointsUpdate(subjectCourse.Id);
tableWorker.EnqueueCoursePointsUpdate(subjectCourse.Id);
tableWorker.EnqueueCoursePointsUpdate(subjectCourse.Id);

await Task.Delay(-1);