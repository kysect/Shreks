using Microsoft.Extensions.Hosting;

namespace Kysect.Shreks.Integration.Google;

public class GoogleTableUpdateWorker : BackgroundService
{
    private static readonly TimeSpan DelayBetweenSheetUpdates = TimeSpan.FromMinutes(1);

    private readonly GoogleTableAccessor _tableAccessor;
    private readonly TableUpdateQueue _tableUpdateQueue;

    public GoogleTableUpdateWorker(GoogleTableAccessor tableAccessor, TableUpdateQueue tableUpdateQueue)
    {
        _tableAccessor = tableAccessor;
        _tableUpdateQueue = tableUpdateQueue;
    }

    protected override async Task ExecuteAsync(CancellationToken token)
    {
        using var timer = new PeriodicTimer(DelayBetweenSheetUpdates);
        while (!token.IsCancellationRequested && await timer.WaitForNextTickAsync(token))
        {
            var pointsUpdateTasks = _tableUpdateQueue
                .PointsUpdateSubjectCourseIds
                .GetAndClearValues()
                .Select(i => _tableAccessor.UpdatePointsAsync(i, token));

            var queueUpdateTasks = _tableUpdateQueue
                .QueueUpdateSubjectCourseIds
                .GetAndClearValues()
                .Select(i => _tableAccessor.UpdateQueueAsync(i, token));

            await Task.WhenAll(pointsUpdateTasks.Concat(queueUpdateTasks));
        }
    }
}