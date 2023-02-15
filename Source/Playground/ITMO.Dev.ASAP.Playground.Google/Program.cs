using ITMO.Dev.ASAP.Application.Abstractions.Google;
using ITMO.Dev.ASAP.Application.Google.Workers;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.DataAccess.Context;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace ITMO.Dev.ASAP.Playground.Google;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .CreateLogger();

        IServiceProvider services = await PlaygroundServices.BuildServiceProvider();

        await services.EnsureDatabaseSeeded();

        DatabaseContext databaseContext = services.GetRequiredService<DatabaseContext>();

        ITableUpdateQueue tableQueue = services.GetRequiredService<ITableUpdateQueue>();
        GoogleTableUpdateWorker tableWorker = services.GetRequiredService<GoogleTableUpdateWorker>();

        await tableWorker.StartAsync(default);

        SubjectCourse subjectCourse = databaseContext.SubjectCourses.First();
        StudentGroup group = subjectCourse.Groups.First().StudentGroup;

        tableQueue.EnqueueCoursePointsUpdate(subjectCourse.Id);
        tableQueue.EnqueueSubmissionsQueueUpdate(subjectCourse.Id, group.Id);

        StudentGroup sameCourseGroup = subjectCourse.Groups.Skip(1).First().StudentGroup;
        tableQueue.EnqueueSubmissionsQueueUpdate(subjectCourse.Id, sameCourseGroup.Id);

        subjectCourse.Groups.Take(8).ToList()
            .ForEach(g => tableQueue.EnqueueSubmissionsQueueUpdate(g.SubjectCourseId, g.StudentGroupId));

        await Task.Delay(TimeSpan.FromSeconds(30));

        SubjectCourse anotherSubjectCourse = databaseContext.SubjectCourses.Skip(1).First();
        StudentGroup anotherGroup = anotherSubjectCourse.Groups.First().StudentGroup;
        tableQueue.EnqueueCoursePointsUpdate(anotherSubjectCourse.Id);
        tableQueue.EnqueueSubmissionsQueueUpdate(anotherSubjectCourse.Id, anotherGroup.Id);

        await Task.Delay(TimeSpan.FromMinutes(2));

        tableQueue.EnqueueCoursePointsUpdate(subjectCourse.Id);
        tableQueue.EnqueueSubmissionsQueueUpdate(subjectCourse.Id, group.Id);
        tableQueue.EnqueueCoursePointsUpdate(subjectCourse.Id);
        tableQueue.EnqueueSubmissionsQueueUpdate(anotherSubjectCourse.Id, anotherGroup.Id);

        await Task.Delay(-1);
    }
}