using Bogus;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Application.Abstractions.Google.Models;
using Kysect.Shreks.Application.Abstractions.Google.Queries;
using Kysect.Shreks.Application.Handlers.Google;
using Kysect.Shreks.Core.Formatters;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Context;
using Kysect.Shreks.DataAccess.Extensions;
using Kysect.Shreks.Integration.Google.Extensions;
using Kysect.Shreks.Integration.Google.Providers;
using Kysect.Shreks.Seeding.EntityGenerators;
using Kysect.Shreks.Seeding.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var credential = await GoogleCredential.FromFileAsync("client_secrets.json", default);

var initializer = new BaseClientService.Initializer
{
    HttpClientInitializer = credential
};

var sheetsService = new SheetsService(initializer);

const string spreadsheetId = "";

IServiceProvider services = new ServiceCollection()
    .AddGoogleIntegration()
    .AddSingleton(sheetsService)
    .AddSingleton<IUserFullNameFormatter, UserFullNameFormatter>()
    .AddSingleton<ICultureInfoProvider, RuCultureInfoProvider>()
    .AddEntityGenerators(o => o
        .ConfigureEntityGenerator<Submission>(s => s.Count = 500)
        .ConfigureEntityGenerator<Student>(s => s.Count = 100)
        .ConfigureEntityGenerator<Assignment>(a => a.Count = 50)
        .ConfigureFaker(f => f.Locale = "ru"))
    .AddDatabaseContext(o => o
        .UseLazyLoadingProxies()
        .UseInMemoryDatabase("Data Source=playground.db"))
    .AddDatabaseSeeders()
    .BuildServiceProvider();

var databaseContext = services.GetRequiredService<ShreksDatabaseContext>();
databaseContext.Database.EnsureCreated();
await databaseContext.SaveChangesAsync();
await services.UseDatabaseSeeders();

var googleTableAccessor = services.GetRequiredService<IGoogleTableAccessor>();

var submissionGenerator = services.GetRequiredService<IEntityGenerator<Submission>>();

IReadOnlyCollection<Submission> submissions = services
    .GetRequiredService<Faker>().Random
    .ListItems(submissionGenerator.GeneratedEntities.ToArray());

var queue = new StudentsQueue(submissions);
await googleTableAccessor.UpdateQueueAsync(spreadsheetId, queue);

var coursePointsHandler = new GetCoursePointsBySubjectCourseHandler(databaseContext);
var coursePointsQuery = new GetCoursePointsBySubjectCourse.Query(databaseContext.SubjectCourses.First().Id);
var points = await coursePointsHandler.Handle(coursePointsQuery, default);
    
await googleTableAccessor.UpdatePointsAsync(spreadsheetId, points.Points);