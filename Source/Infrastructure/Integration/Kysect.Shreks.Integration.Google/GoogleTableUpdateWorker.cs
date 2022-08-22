using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kysect.Shreks.Integration.Google;

public class GoogleTableUpdateWorker : BackgroundService
{
    private static readonly TimeSpan DelayBetweenSheetUpdates = TimeSpan.FromMinutes(1);

    private readonly TableUpdateQueue _tableUpdateQueue;
    private readonly IServiceScopeFactory _serviceProvider;

    public GoogleTableUpdateWorker(TableUpdateQueue tableUpdateQueue, IServiceScopeFactory serviceProvider)
    {
        _tableUpdateQueue = tableUpdateQueue;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken token)
    {
        using var timer = new PeriodicTimer(DelayBetweenSheetUpdates);
        while (!token.IsCancellationRequested && await timer.WaitForNextTickAsync(token))
        {
            using IServiceScope serviceScope = _serviceProvider.CreateScope();
            using var googleTableAccessor = serviceScope.ServiceProvider.GetRequiredService<GoogleTableAccessor>();
            
            var pointsUpdateTasks = _tableUpdateQueue
                .PointsUpdateSubjectCourseIds
                .GetAndClearValues()
                .Select(c => googleTableAccessor.UpdatePointsAsync(c, token));

            var queueUpdateTasks = _tableUpdateQueue
                .QueueUpdateSubjectCourseGroupIds
                .GetAndClearValues()
                .Select(t =>
                {
                    var (courseId, groupId) = t;
                    return googleTableAccessor.UpdateQueueAsync(courseId, groupId, token);
                });

            await Task.WhenAll(pointsUpdateTasks.Concat(queueUpdateTasks));
        }
    }
}