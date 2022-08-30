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
        // TODO: return to queue on fail
        using var timer = new PeriodicTimer(DelayBetweenSheetUpdates);
        while (!token.IsCancellationRequested && await timer.WaitForNextTickAsync(token))
        {
            using IServiceScope serviceScope = _serviceProvider.CreateScope();
            using var googleTableAccessor = serviceScope.ServiceProvider.GetRequiredService<GoogleTableAccessor>();

            IReadOnlyCollection<Guid> points = _tableUpdateQueue
                .PointsUpdateSubjectCourseIds
                .GetAndClearValues();

            foreach (Guid point in points)
                await googleTableAccessor.UpdatePointsAsync(point, token);

            IReadOnlyCollection<(Guid, Guid)> queues = _tableUpdateQueue
                .QueueUpdateSubjectCourseGroupIds
                .GetAndClearValues();

            foreach ((Guid courseId, Guid groupId) in queues)
                await googleTableAccessor.UpdateQueueAsync(courseId, groupId, token);
        }
    }
}