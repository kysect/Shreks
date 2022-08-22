using Kysect.Shreks.Application.Handlers.Extensions;
﻿using Google.Apis.Auth.OAuth2;
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
    .AddGoogleIntegration(o => o
        .ConfigureGoogleCredentials(googleCredentials)
        .ConfigureDriveId("17CfXw__b4nnPp7VEEgWGe-N8VptaL1hP"))
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