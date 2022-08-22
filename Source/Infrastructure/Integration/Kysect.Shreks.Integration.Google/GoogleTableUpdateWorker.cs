using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kysect.Shreks.Integration.Google;

public class GoogleTableUpdateWorker : BackgroundService
{
    private static readonly TimeSpan DelayBetweenSheetUpdates = TimeSpan.FromMinutes(1);

    private readonly TableUpdateQueue _tableUpdateQueue;
    private readonly IServiceScopeFactory _serviceProvider;

    internal GoogleTableUpdateWorker(TableUpdateQueue tableUpdateQueue, IServiceScopeFactory serviceProvider)
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
                .Select(i => googleTableAccessor.UpdatePointsAsync(i, token));

            var queueUpdateTasks = _tableUpdateQueue
                .QueueUpdateSubjectCourseIds
                .GetAndClearValues()
                .Select(i => googleTableAccessor.UpdateQueueAsync(i, token));

            await Task.WhenAll(pointsUpdateTasks.Concat(queueUpdateTasks));
        }
    }
}