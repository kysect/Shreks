using Kysect.Shreks.Application.Handlers.Extensions;
using Google.Apis.Auth.OAuth2;
using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Application.Extensions;
using Kysect.Shreks.Application.TableManagement;
using Kysect.Shreks.DataAccess.Context;
using Kysect.Shreks.Integration.Google;
using Kysect.Shreks.Integration.Google.Extensions;
using Kysect.Shreks.Mapping.Extensions;
using Kysect.Shreks.Playground.Google;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

var googleCredentials = await GoogleCredential.FromFileAsync("client_secrets.json", default);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

IServiceProvider services = new ServiceCollection()
    .AddApplicationConfiguration()
    .AddGoogleIntegration(o => o
        .ConfigureGoogleCredentials(googleCredentials)
        .ConfigureDriveId("17CfXw__b4nnPp7VEEgWGe-N8VptaL1hP"))
    .AddHandlers()
    .AddMappingConfiguration()
    .AddLogging(o => o.AddSerilog())
    .AddSingleton<GoogleTableUpdateWorker>()
    .AddGooglePlaygroundDatabase()
    .BuildServiceProvider();

await services.EnsureDatabaseSeeded();

var databaseContext = services.GetRequiredService<ShreksDatabaseContext>();

var tableQueue = services.GetRequiredService<ITableUpdateQueue>();
var tableWorker = services.GetRequiredService<GoogleTableUpdateWorker>();

await tableWorker.StartAsync(default);

var subjectCourse = databaseContext.SubjectCourses.First();
var group = subjectCourse.Groups.First().StudentGroup;

tableQueue.EnqueueCoursePointsUpdate(subjectCourse.Id);
tableQueue.EnqueueSubmissionsQueueUpdate(subjectCourse.Id, group.Id);

var sameCourseGroup = subjectCourse.Groups.Skip(1).First().StudentGroup;
tableQueue.EnqueueSubmissionsQueueUpdate(subjectCourse.Id, sameCourseGroup.Id);

subjectCourse.Groups.Take(8).ToList().ForEach(g => tableQueue.EnqueueSubmissionsQueueUpdate(g.SubjectCourseId, g.StudentGroupId));

await Task.Delay(TimeSpan.FromSeconds(30));

var anotherSubjectCourse = databaseContext.SubjectCourses.Skip(1).First();
var anotherGroup = anotherSubjectCourse.Groups.First().StudentGroup;
tableQueue.EnqueueCoursePointsUpdate(anotherSubjectCourse.Id);
tableQueue.EnqueueSubmissionsQueueUpdate(anotherSubjectCourse.Id, anotherGroup.Id);

await Task.Delay(TimeSpan.FromMinutes(2));

tableQueue.EnqueueCoursePointsUpdate(subjectCourse.Id);
tableQueue.EnqueueSubmissionsQueueUpdate(subjectCourse.Id, group.Id);
tableQueue.EnqueueCoursePointsUpdate(subjectCourse.Id);
tableQueue.EnqueueSubmissionsQueueUpdate(anotherSubjectCourse.Id, anotherGroup.Id);

await Task.Delay(-1);